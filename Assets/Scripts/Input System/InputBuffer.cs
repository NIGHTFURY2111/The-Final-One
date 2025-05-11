using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

#region InputStruct defination
struct InputStruct
{
    public string Name { get; private set; }
    public object Value { get; private set; }
    public float InputTime{ get; private set; }
    public float BufferTime { get; private set; }
    public bool Used { get; private set; }
    public bool IsBuffered;
    /// <summary>
    /// Checks if the input is expired based on its buffer time.
    /// </summary>
    public bool ChackIfExpired => Time.time - InputTime > BufferTime;

    public InputStruct(string name, object value, float bufferTime)
    {
        Name = name;
        InputTime = Time.time;
        Value = value;
        Used = false;
        IsBuffered = true;
        BufferTime = bufferTime;
    }

    public InputStruct(string name, object value)
    {
        Name = name;
        InputTime = Time.time;
        Value = value;
        Used = false;
        IsBuffered = false;
        BufferTime = float.NaN;
    }


    public void UpdateValue(object value)
    {
        if (IsBuffered)
        {
            InputTime = Time.time;
        }
        Value = value;
        Used = false;
    }

    

    public void UpdateBufferTime(float newBufferTime)
    {
        BufferTime = newBufferTime;
        IsBuffered = BufferTime != float.NaN;
    }

    public void MarkAsUsed()
    {
        if (IsBuffered){
        Used = true;
        }

        else{
            Debug.LogWarning("Input is not buffered, cannot be marked as used.");
        }
    }

}
#endregion

public class InputBuffer
{

    private Dictionary<string, InputStruct> buffer = new ();

    /// <summary>
    /// Adds or updates an input in the buffer with a specific buffer time.
    /// </summary>
    public void AddInput(string name, object value, float bufferTime)
    {
        if (buffer.ContainsKey(name))
        {
            // Retrieve the struct, modify it, and update it back in the dictionary
            InputStruct input = buffer[name];
            input.UpdateValue(value);
            input.UpdateBufferTime(bufferTime); // Optionally update buffer time
            buffer[name] = input; // Update the struct back in the dictionary
        }
        else
        {
            // Add new input
            buffer[name] = new InputStruct(name, value, bufferTime);
        }
    }

    /// <summary>
    /// Gets an input from the buffer if it exists and hasn't expired.
    /// </summary>
    public bool GetInput(string name, out object value)
    {
        if (buffer.ContainsKey(name))
        {
            InputStruct input = buffer[name];

            if (!input.Used && !input.ChackIfExpired)
            {
                value = input.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Marks the input as used without reading its value.
    /// </summary>
    public bool MarkInputAsUsed(string name)
    {
        if (!buffer.TryGetValue(name, out InputStruct input))
            return false; // Input does not exist

        if (input.ChackIfExpired)
            return false; // Input expired, cannot be marked as used

        input.MarkAsUsed(); // Modify the struct
        buffer[name] = input; // Update the struct back in the dictionary

        return true; // Successfully marked as used
    }


    /// <summary>
    /// Clears the entire buffer.
    /// </summary>
    public void Clear()
    {
        buffer.Clear();
    }

    /// <summary>
    /// Lists all inputs in the buffer (for debugging purposes).
    /// </summary>
    public string List()
    {
        string outp = "";
        foreach (var kvp in buffer)
        {
            var input = kvp.Value;
            outp += $"{input.Name}\t{input.Value}\t{input.Used}\t{input.InputTime}\t{input.BufferTime}\n";
            //outp += kvp.ToString()+"\n";
        }
        return outp;
    }
}

