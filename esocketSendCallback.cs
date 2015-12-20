using System.Collections;
using System;
using System.Net.Sockets;
using UnityEngine;

public class esocketSendCallback {
	private AsyncCallback acb;

	public esocketSendCallback(AsyncCallback acb) {
		this.acb = acb;
	}

	public void Callback(IAsyncResult iar) {
		Socket so = (Socket)iar.AsyncState;
		int n = so.EndSend (iar);

		Debug.Log ("esocketSendCallback : " + n + " bytes sended");

		acb (iar);
	}
}
