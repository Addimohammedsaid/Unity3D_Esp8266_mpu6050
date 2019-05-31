using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

//This Scripts is attach to a 3D ball 
public class BallBehaviour : MonoBehaviour
{
    //Ball Parameter 
    public float speed;
    public float jump;
    Rigidbody rb;
    float xAxis,zAxis;
    string[] container;
    float[]  rotation = {0,0,0};
    
    //TCP Init :
    public string IP = "192.168.1.100";
    public int Port = 1234;
    public byte[] data;
    public Socket client;
 
    //Get Position :
    void getPosition (string position){
      container = position.Split(',');
      for(int i=0;i<container.Length-1;i++){
        rotation[i] = float.Parse(container[i])/1000;
      }
    }

    //TCP Function :

    //Make Connection
    public void connect(){        
        client = new Socket (AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);        
        client.Connect(IP,Port);
    }

    //Send Message 
    void send(){
        data = System.Text.Encoding.ASCII.GetBytes("GyZ \nGyY \nGyX");
        client.Send(data);
    }

    //Receive function 
    void receive(){
        byte[] b = new byte[1024];        
        int k = client.Receive(b);
        string Received = System.Text.Encoding.ASCII.GetString(b,0,k);                
        if(client.Connected){                        
            Debug.Log(Received);
            getPosition(Received);
        }
        else {
            Debug.Log("Not connected");
        }
        client.Close();
    }

    // Start is called before the first frame update
    void Start()
    {      
      rb = GetComponent<Rigidbody>();  
    }

    void update (){
      // Crash after 10 connection
        connect();
        send();    
        receive();  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(rotation[0],rotation[1],rotation[2]);  
        //rb.velocity = new Vector3(0,rb.velocity.y,speed);   
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
        rb.velocity = new Vector3(xAxis * speed , rb.velocity.y , zAxis * speed);

      if(Input.GetKeyDown(KeyCode.Space)){
        rb.velocity  = new Vector3(xAxis * speed , jump , zAxis * speed);
      }
    }
}
