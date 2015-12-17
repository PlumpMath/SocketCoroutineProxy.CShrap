using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;

public class test : MonoBehaviour {
	//esocket es;
	// Use this for initialization
	void Start () {
		esocket es = new esocket ("127.0.0.1", 9988);

		//SocketProxy spt = new SocketProxy (ts (), es);
		SocketProxy spr = new SocketProxy (rs (), es);
	
		//StartCoroutine (spt.Proxy());
		StartCoroutine (spr.Proxy());
	}

	void ConnectCB(IAsyncResult iar) {
		Debug.Log("test : Connect finished");

		Socket so = (Socket)iar.AsyncState;

		if (so.Connected) {
			Debug.Log ("ConnectSucc called");
		} else {
			Debug.Log ("ConnectFailed called");
		}
	}
		
	byte[] GetBytes(Vector3 v3) {
		byte[] bs = new byte[12];
		BitConverter.GetBytes (v3.x).CopyTo(bs, 0);
		BitConverter.GetBytes (v3.y).CopyTo(bs, 4);
		BitConverter.GetBytes (v3.z).CopyTo(bs, 8);
		return bs;
	}

	Vector3 ToVector3(byte[] bs, int offset) {
		float x = BitConverter.ToSingle (bs, offset);
		float y = BitConverter.ToSingle (bs, offset + 4);
		float z = BitConverter.ToSingle (bs, offset + 8);
		return new Vector3 (x, y, z);
	}

	string GetPostionBase64() {
		byte[] bs = new byte[48];
		GetBytes (RC.left [0].transform.position).CopyTo (bs, 0);
		GetBytes (RC.left [1].transform.position).CopyTo (bs, 12);
		GetBytes (RC.right [0].transform.position).CopyTo (bs, 24);
		GetBytes (RC.right [1].transform.position).CopyTo (bs, 36);
		return Convert.ToBase64String (bs);
	}

	void SetPostionByBase64(string b64) {
		byte[] bs = Convert.FromBase64String (b64);
		RC.left [0].transform.position = ToVector3 (bs, 0);
		RC.left [1].transform.position = ToVector3 (bs, 12);
		RC.right [0].transform.position = ToVector3 (bs, 24);
		RC.right [1].transform.position = ToVector3 (bs, 36);
	}

	IEnumerator ts() {
		while (true) {
			if (RC.left[0] == null)
				yield return null;
			//es.Send(GetPostionBase64());
			yield return new WaitForSeconds(1);
		}
	}

	IEnumerator rs() {
		while (true) {
			ZSocketSignal signal = new ZSocketSignal (ZSocketSignal.Signals.Recv);

			yield return signal;

			Debug.Log (signal.Signal);

			if (signal.Signal == ZSocketSignal.Signals.RecvSuccessful) {
				string[] ds = signal.Parse ();

				if (ds [0] == "SYNC") {
					SetPostionByBase64 (ds [1]);
				}

				Debug.Log ("test Cmd: " + ds [0]);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
