using UnityEngine;
using System.Collections;
using System;

public class ZSocketSignal {
	public SocketSignals Signal {
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
	private SocketSignals realSignal;

	public void Update(SocketSignals sig, string cmdAndArg) {
		realSignal = sig;
		realValue = ProtocolCmd.Parse (cmdAndArg);
	}

	public void Update(SocketSignals sig, ProtocolCmd pcmd) {
		realSignal = sig;
		realValue = pcmd;
	}

	public void Update(SocketSignals sig) {
		realSignal = sig;
		realValue = null;
	}

	public ZSocketSignal(SocketSignals sig) {
		realSignal = sig;
		realValue = null;
	}

	public ZSocketSignal(SocketSignals sig, string cmdAndArg) {
		realSignal = sig;
		realValue = ProtocolCmd.Parse (cmdAndArg);
	}

	public ZSocketSignal(SocketSignals sig, ProtocolCmd pcmd) {
		realSignal = sig;
		realValue = pcmd;
	}
}