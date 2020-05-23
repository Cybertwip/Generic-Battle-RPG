using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Event : MonoBehaviour
{
	private Animator anim;
	private AnimatorClipInfo[] currentClipInfo;
	private AudioClip sfxClip;
	public AudioSource source;

	void Start()
	{
		anim = GetComponent<Animator>();
	}

	public void SetSFX()
	{
		currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
		string ccName = currentClipInfo[0].clip.name;
		Debug.Log(ccName);
		switch (ccName)
		{
			case "Luigi_Jump_start_wSound":
				sfxClip = Resources.Load<AudioClip>("SFX/smrpg_jump");
				PlaySFX();
				break;

			case "Luigi_Jump_end_wSound":
				sfxClip = Resources.Load<AudioClip>("SFX/smrpg_click");
				PlaySFX();
				break;

			case ("Luigi_Walk_wSound"):
				sfxClip = Resources.Load<AudioClip>("SFX/step-floor");
				PlaySFX();
				break;

			case ("Luigi_Run_wSound"):
				sfxClip = Resources.Load<AudioClip>("SFX/step-floor");
				PlaySFX();
				break;

			case ("Luigi_Battle_Punch_wSound"):
				sfxClip = Resources.Load<AudioClip>("SFX/smrpg_mario_shell");
				PlaySFX();
				break;

			case ("Luigi_Battle_PunchTimed_wSound"):
				sfxClip = Resources.Load<AudioClip>("SFX/smrpg_mario_hammer");
				PlaySFX();
				break;

			default:
				Debug.LogError("It's Spencer--This animation clip has an animation event but it can't play the sound you wanted. Check that transition duration is zero, the clip isn't too short, and that your spelling is correct.");
				break;
		}
	}
	
    private void PlaySFX()
	{
		source.PlayOneShot(sfxClip);
	}
}
