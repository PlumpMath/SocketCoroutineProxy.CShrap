using UnityEngine;
using System.Collections;
using System;

public class test : MonoBehaviour {
	esocket es;
	// Use this for initialization
	void Start () {
		es = new esocket ("127.0.0.1", 9988);
		es.Connect (ConnectSucc, ConnectFail);
	
		StartCoroutine ("rs");
		StartCoroutine ("ts");
	}

	void ConnectSucc(IAsyncResult iar) {
		Debug.Log ("ConnectSucc called");
	}

	void ConnectFail(IAsyncResult iar) {
		Debug.Log ("ConnectFail called");
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
			bool esDatasWithNull = true;

			//Debug.Log (es.datas.Count);

			lock (es.datas) {
				if (es.datas.Count > 0) {
					esDatasWithNull = false;
				}
			}
			if (esDatasWithNull) {
				yield return null;
			} else {
				lock (es.datas) {
					Debug.Log ("ts : " + es.datas.First.Value);
					string data = es.datas.First.Value;
					es.datas.RemoveFirst ();

					string[] ds = data.Split (new string[] { " " }, StringSplitOptions.None);

					if (ds [0] == "SYNC") {
						SetPostionByBase64 (ds [1]);
					}

					Debug.Log (ds [0]);
					
					Debug.Log (es.datas.Count);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
