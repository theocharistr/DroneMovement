using UnityEngine;
using System;
using System.Data;
using System.IO;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.XR;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using static Movement;

public class Movement : MonoBehaviour
{
    double lat, lat2;
    double lon, lon2;
    double alt, alt2;
    string drone_id;
    private HttpListener listener;
    private Thread listenerThread;
    public GameObject Drone;
    void Start()
    {

        listener = new HttpListener();
        listener.Prefixes.Add("http://160.40.54.171:5052/send/");
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Start();
        listenerThread = new Thread(startListener);
        listenerThread.Start();
        Debug.Log("Server Started");
    }
    private void startListener()
    {
        while (true)
        {
            var result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne();
        }
    }

    private void ListenerCallback(IAsyncResult result)
    {
        var context = listener.EndGetContext(result);

        Debug.Log("Method: " + context.Request.HttpMethod);
        Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);

        if (context.Request.QueryString.AllKeys.Length > 0)
            foreach (var key in context.Request.QueryString.AllKeys)
            {
                Debug.Log("Key: " + key + ", Value: " + context.Request.QueryString.GetValues(key)[0]);
            }

        if (context.Request.HttpMethod == "POST")
        {
            Thread.Sleep(200);
            var data_text = new StreamReader(context.Request.InputStream,
                                context.Request.ContentEncoding).ReadToEnd();
            context.Response.StatusCode = 200;
            byte[] buffer = Encoding.UTF8.GetBytes(data_text);
            context.Response.ContentLength64 = buffer.Length;
            using Stream ros = context.Response.OutputStream;
            ros.Write(buffer, 0, buffer.Length);

            //Send to make action
            createAction(data_text);
        }
        context.Response.Close();
    }
    void Update()
    {

        switch (drone_id)
        {
            case "UAV1":
                Debug.Log("The drone is a UAV1");
                transform.position = Quaternion.AngleAxis((float)lon, -Vector3.up) * Quaternion.AngleAxis((float)lat, -Vector3.right) * new Vector3(0, 0, 1);
                transform.position += new Vector3(0, (float)alt, 0);
                break;
            case "UAV2":
                GameObject drone2 = Instantiate(Drone, new Vector3(0, 0, 0), Quaternion.identity);
                Debug.Log("The drone is a UAV2");
                drone2.transform.position = Quaternion.AngleAxis((float)lon, -Vector3.up) * Quaternion.AngleAxis((float)lat, -Vector3.right) * new Vector3(0, 0, 1);
                drone2.transform.position += new Vector3(0, (float)alt, 0);
                break;
            case "UAV3":
                GameObject drone3 = Instantiate(Drone, new Vector3(0, 0, 0), Quaternion.identity);
                Debug.Log("The drone is a UAV2");
                drone3.transform.position = Quaternion.AngleAxis((float)lon, -Vector3.up) * Quaternion.AngleAxis((float)lat, -Vector3.right) * new Vector3(0, 0, 1);
                drone3.transform.position += new Vector3(0, (float)alt, 0);
                break;
            default:

                Debug.Log("The drone is unknown");
                break;
        }
        /*  Transform transform2 = GameObject.FindGameObjectWithTag("Drone2").transform; 
            transform2.position = Quaternion.AngleAxis((float)lon2, -Vector3.up) * Quaternion.AngleAxis((float)lat2, -Vector3.right) * new Vector3(0, 0, 1);
            transform2.position += new Vector3(0, (float)alt2, 0);
            Debug.Log(transform.position + "transform.position of Drone 1" + transform2.position + "transform.position of Drone 2"); 
    */
    }

    void createAction(string data)
    {
        //json format 
        //{ "DRONE_ID": "string", "GPS.LATITUDE": double, "GPS.LONGITUDE": double, "BAROMETER": double, "TIMECODE": double}
        Debug.Log(data);

        JObject joResponse = JObject.Parse(data);
        drone_id = (string)joResponse["DRONE_ID"];
        lat = (double)joResponse["GPS.LATITUDE"];
        lon = (double)joResponse["GPS.LONGITUDE"];
        alt = (double)joResponse["ALTITUDE"];
        //id = drone_id;
        //print(drone_id + " droneid");
        /* if (drone_id == "UAV2")
        // {
             lat2 = (double)joResponse["GPS.LATITUDE"];
             lon2 = (double)joResponse["GPS.LONGITUDE"];
             alt2 = (double)joResponse["ALTITUDE"];
        } */
    }
}
