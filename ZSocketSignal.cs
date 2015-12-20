using UnityEngine;
using System.Collections;
using System;

public class ZSocketSignal {
	public enum Signals
	{
		ConnectFailed,
		ConnectSuccessful,
		RecvSuccessful,
		RecvFailed,
		SendSuccessful,
		SendFailed,
		Dead,

		Connect,
		ConnectBlock,
		Recv,
		Send,
		SendBlock
	}

	public Signals Signal {
		get {
			return realSignal;
		}
	}

	public string ValueString {
		get {
			if (realValue == null)
				return "";
			return realValue.ToString ();
		}
	}

	public ProtocolCmd Value {
		get {
			if (realValue == null) {
				return ProtocolCmd.Null;
			}
			return realValue;
		}
	}

	private ProtocolCmd realValue;
	private Signals realSignal;

	public void Update(Signals sig, string cmdAndArg) {
		realSignal = sig;
		realValue = ProtocolCmd.Parse (cmdAndArg);
	}

	public void Update(Signals sig, ProtocolCmd pcmd) {
		realSignal = sig;
		realValue = pcmd;
	}

	public void Update(Signals sig) {
		realSignal = sig;
		realValue = null;
	}

	public ZSocketSignal(Signals sig) {
		realSignal = sig;
		realValue = null;
	}

	public ZSocketSignal(Signals sig, string cmdAndArg) {
		realSignal = sig;
		realValue = ProtocolCmd.Parse (cmdAndArg);
	}

	public ZSocketSignal(Signals sig, ProtocolCmd pcmd) {
		realSignal = sig;
		realValue = pcmd;
	}
}