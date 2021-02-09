using UnityEngine;
using System.Collections;

public class SuckItPaul : MonoBehaviour {
	public AudioClip SuckItSound;
	public void PlaySuckItPaul()
	{
		AudioSource.PlayClipAtPoint (SuckItSound, this.transform.position);
	}
	
}
