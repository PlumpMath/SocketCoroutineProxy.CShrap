using UnityEngine;
using System.Collections;

public class t2 : MonoBehaviour {

	// This function represents a generator of the fibonacci sequence
	IEnumerator Fibonacci(){
		int Fkm2 = 1;
		int Fkm1 = 1;
		yield return 1; // The first two values are 1
		yield return 1;
		// Now, each time we continue execution, generate the next entry.
		while(Fkm1 + Fkm2 < int.MaxValue){
			int Fk = Fkm2 + Fkm1;
			Fkm2 = Fkm1;
			Fkm1 = Fk;
			yield return Fk;
		}
	}

	void Start () {
		// Call the Fibonacci function, which 
		// immediately returns an IEnumerator
		// (No code in Fibonnacci is run)
		IEnumerator fib = Fibonacci();

		// Generate the first 10 Fibonacci numbers
		for(int i = 0; i < 10; i++){
			// MoveNext runs the Fibonacci function
			// with the stored stack frame in the Ienumerator
			if(!fib.MoveNext())
				break;
			// Current returns the value that was yielded
			Debug.Log((int)fib.Current);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
