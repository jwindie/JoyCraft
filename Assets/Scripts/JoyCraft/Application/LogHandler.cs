using System;
using System.IO;
using UnityEngine;

public class LogHandler {

    private bool streamOpen;
    private string logPath;
    private StreamWriter stream;
    private bool logToUnity = false;

    public LogHandler(string path) {
        Debug.Log ("CANT WRITE FILES ON THIS MAC. CONTACT ADMIN");
        return;
        NewStream(path);
    }

    public void Log(string message) {
        if (streamOpen) stream.WriteLine($"{GetTimeLabel()} {message}");
        if (logToUnity) UnityEngine.Debug.Log(message);
    }

    public void Append (string message) {
        if (streamOpen) stream.Write (message);
    }

    public void Space () {
        if (streamOpen) stream.WriteLine();
    }

    public void OpenLogFile() {
        System.Diagnostics.Process.Start (logPath);
    }

    public void LogToUnity(bool state) {
        logToUnity = state;
    }

    public void Clsoe() {
        if (streamOpen) {
            stream.Flush();
            stream.Close();
            streamOpen = false;
        }
    }

    private void NewStream(string path) {
        if (streamOpen) {
            stream.Flush();
            stream.Close();
        }

        //open a new stream
        stream = new StreamWriter (logPath = path);
        stream.AutoFlush = true;
        streamOpen = true;
    }

    private string GetTimeLabel() {
        return $"[{DateTime.Now.ToLongTimeString()}]";
    }

    //purpose:
        //create logFile
        //write to log file
        //open log file in case of error or crash

}