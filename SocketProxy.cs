using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;

public class SocketProxy {
	private bool Connected;
	private bool ConnectFinished;
	private bool Sended;

	private IEnumerator ie;
	//private esocket es;
	private SocketCallback Callback;

	public delegate void SocketCallback(ZSocketSignal zsc);

	public SocketProxy(IEnumerator ie, SocketCallback Callback) {
		this.ie = ie;
		this.Callback = Callback;
		//this.es = new esocket ();
	}

	void SendCallback(IAsyncResult iar) {
		Sended = true;
	}

	void ConnectCallback(IAsyncResult iar) {
		Socket so = (Socket)iar;
		ConnectFinished = true;
		Connected = so.Connected;
		if (Connected) {
			Callback (new ZSocketSignal (ZSocketSignal.Signals.ConnectSuccessful, ""));
		} else {
			Callback (new ZSocketSignal (ZSocketSignal.Signals.ConnectFailed, ""));
		}
	}

	public IEnumerator Proxy() {
		while (true) {
			if (!ie.MoveNext ()) {
				yield break;
			}

			object yielded = ie.Current;

			if (yielded != null && yielded.GetType () == typeof(ZSocketSignal)) {
				ZSocketSignal Sig = (ZSocketSignal)yielded;
				switch (Sig.Signal) {
				case ZSocketSignal.Signals.Connect:
					
					break;

				case ZSocketSignal.Signals.Recv:
					while (true) {
						bool esDatasWithNull = true;

						lock (es.datas) {
							if (es.datas.Count > 0) {
								esDatasWithNull = false;
							}
						}
						if (esDatasWithNull) {
							yield return null;
						} else {
							lock (es.datas) {
								string data = es.datas.First.Value;
								es.datas.RemoveFirst ();
								Callback (new ZSocketSignal (ZSocketSignal.Signals.Recv, data));
							}
							break;
						}
					}
					break;

				case ZSocketSignal.Signals.Send:
					es.Send (Sig.Value);
					yield return null;
					break;

				case ZSocketSignal.Signals.SendBlock:
					es.SendCallback (Sig.Value, SendCallback);
					Sended = false;
					while (!Sended) {
						yield return null;
					}
					break;
				}
			}
		}
	}
}
