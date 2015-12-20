using UnityEngine;
using System.Collections;
using System;

public class ProtocolCmd {
	public class safeStringVector {
		private string[] realArgs;

		public string this[int index] {
			get {
				if (realArgs == null || index < 0 || index >= realArgs.Length) {
					return "";
				} else {
					return realArgs [index];
				}
			}
		}

		public safeStringVector(string[] ss) {
			realArgs = ss;
		}

		public override string ToString() {
			if (realArgs == null) {
				return "";
			}
			return String.Join (" ", realArgs);
		}
	}

	public readonly static ProtocolCmd Null = new ProtocolCmd("");

	public readonly safeStringVector Args;
	public readonly string Cmd;

	public static ProtocolCmd Parse(string raw) {
		string[] s = raw.Split (new string[] { " " }, StringSplitOptions.None);

		if (s.Length == 0) {
			return new ProtocolCmd ("", (string[])(null));	
		}
		if (s.Length == 1) {
			return new ProtocolCmd (s[0], (string[])(null));
		}
		string[] s2 = new string[s.Length-1];
		for (int i = 0; i < s.Length - 1; i++) {
			s2 [i] = s [i + 1];
		}
		return new ProtocolCmd(s[0], s2);
	}

	public ProtocolCmd(string cmd, string[] args) {
		this.Args = new safeStringVector (args);
		this.Cmd = cmd;
	}

	public ProtocolCmd(string cmd) {
		this.Cmd = cmd;
		this.Args = new safeStringVector (null);
	}

	public ProtocolCmd(string cmd, string arg1) {
		this.Cmd = cmd;

		string[] args = new string[1];
		args [0] = arg1;
		this.Args = new safeStringVector (args);

	}

	public ProtocolCmd(string cmd, string arg1, string arg2) {
		this.Cmd = cmd;

		string[] args = new string[2];
		args [0] = arg1;
		args [1] = arg2;
		this.Args = new safeStringVector (args);
	}

	public ProtocolCmd(string cmd, string arg1, string arg2, string arg3) {
		this.Cmd = cmd;

		string[] args = new string[3];
		args [0] = arg1;
		args [1] = arg2;
		args [2] = arg3;
		this.Args = new safeStringVector (args);
	}

	public override string ToString() {
		return Cmd + " " + Args;
	}
}
