using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Event : MonoBehaviour
{
	private Animator anim;
	private AnimatorClipInfo[] currentClipInfo;
	private AudioClip sfxClip;
	private GameObject audioManager;
	private List<string> anims_w_soundFX;
	public AudioSource source;

	void Start()
	{
		anim = GetComponent<Animator>();
		audioManager = GameObject.FindGameObjectWithTag("AudioManager");
		anims_w_soundFX = audioManager.GetComponent<AudioManager>().anims_w_soundFX;
	}
	public void SetSFX()
	{
		currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
		string ccName = currentClipInfo[0].clip.name;
		if (anims_w_soundFX.Contains(ccName))
		{
			switch (ccName)
			{
				case "Luigi_Jump_start_wSound":
					sfxClip = Resources.Load<AudioClip>("SFX/smrpg_jump");
					PlaySFX();
					break;

				case "Luigi_Jump_down_wSound":
					sfxClip = Resources.Load<AudioClip>("SFX/freesfx_falling_quick");
					PlaySFX();
					break;

				case "Luigi_Jump_end_wSound": // fix this
					sfxClip = Resources.Load<AudioClip>("SFX/smrpg_click");
					PlaySFX();
					break;

				case "Luigi_Walk_wSound":
					sfxClip = Resources.Load<AudioClip>("SFX/step-floor");
					PlaySFX();
					break;

				case "Luigi_Run_wSound":
					sfxClip = Resources.Load<AudioClip>("SFX/step-floor");
					PlaySFX();
					break;

				case "Luigi_Battle_Punch_wSound":
					sfxClip = Resources.Load<AudioClip>("SFX/smrpg_mario_shell");
					PlaySFX();
					break;

				case "Luigi_Battle_PunchTimed_wSound":
					sfxClip = Resources.Load<AudioClip>("SFX/smrpg_mario_hammer");
					PlaySFX();
					break;

				default:
					Debug.LogError("Something went wrong finding the sfxClip in the cases. Check spelling");
					break;
			}
		}
		else
		{
			Debug.LogError(ccName + " isn't known to the AudioManager. Add it to the list in AudioManger.cs");
		}
	}

	private void PlaySFX()
	{
		source.PlayOneShot(sfxClip);
	}
}