using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

#region InputStruct
// Represents a single input entry in the buffer
class InputStruct
{
    public string Name { get; private set; }
    public object Value { get; private set; }
    public float InputTime { get; private set; }
    public float BufferTime { get; private set; }
    public bool Used { get; private set; }
    public bool IsBuffered { get; private set; }

    // Determines if the buffered input has expired
    public bool IsExpired => !IsBuffered ? false : (Time.time - InputTime > BufferTime);

    /// <summary>
    /// Initializes a new instance of the InputStruct class.
    /// </summary>
    public InputStruct(string name, object value, float bufferTime)
    {
        Name = name;
        InputTime = Time.time;
        Value = value;
        Used = false;
        IsBuffered = !float.IsNaN(bufferTime);
        BufferTime = bufferTime;
    }

    /// <summary>
    /// Updates the value and resets the input time if buffered.
    /// </summary>
    public void UpdateValue(object value)
    {
        if (IsBuffered)
            InputTime = Time.time;
        Value = value;
        Used = false;
    }

    /// <summary>
    /// Updates the buffer time and whether the input is buffered.
    /// </summary>
    public void UpdateBufferTime(float newBufferTime)
    {
        BufferTime = newBufferTime;
        IsBuffered = !float.IsNaN(BufferTime);
    }

    /// <summary>
    /// Marks the input as used if it is buffered.
    /// </summary>
    public void MarkAsUsed()
    {
        if (IsBuffered)
            Used = true;
        else
            Debug.LogWarning("Input is not buffered, cannot be marked as used.");
    }
}
#endregion

#region InputBuffer
// Manages a buffer of input entries
public class InputBuffer
{
    private readonly Dictionary<string, InputStruct> buffer = new();

    /// <summary>
    /// Adds or updates an input in the buffer.
    /// </summary>
    public void AddInput(string name, object value, float bufferTime = float.NaN)
    {
        if (buffer.TryGetValue(name, out var input))
        {
            input.UpdateValue(value);
            input.UpdateBufferTime(bufferTime);
        }
        else
        {
            buffer[name] = new InputStruct(name, value, bufferTime);
        }
    }

    /// <summary>
    /// Retrieves an input value if it exists, is not used, and is not expired.
    /// </summary>
    public bool GetInput(string name, out object value)
    {
        if (buffer.TryGetValue(name, out var input) && !input.Used && !input.IsExpired)
        {
            value = input.Value;
            return true;
        }
        value = null;
        return false;
    }

    /// <summary>
    /// Marks an input as used if it exists and is not expired.
    /// </summary>
    public bool MarkInputAsUsed(string name)
    {
        if (!buffer.TryGetValue(name, out var input) || input.IsExpired)
            return false;
        input.MarkAsUsed();
        return true;
    }

    /// <summary>
    /// Clears the entire input buffer.
    /// </summary>
    public void Clear() => buffer.Clear();

    /// <summary>
    /// Returns a string listing all buffered inputs.
    /// </summary>
    public string List()
    {
        var outp = "";
        foreach (var kvp in buffer)
        {
            var input = kvp.Value;
            outp += $"{input.Name}\t{input.Value}\t{input.Used}\t{input.InputTime}\t{input.BufferTime}\n";
        }
        return outp;
    }
}

#endregion
