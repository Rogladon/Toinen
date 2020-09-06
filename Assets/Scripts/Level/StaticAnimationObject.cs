using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAnimationObject : MonoBehaviour
{

	DragonBones.UnityArmatureComponent armature;

	//[SerializeField]
	//private StringIntDictionary _animations = StringIntDictionary.New<StringIntDictionary>();
	//public Dictionary<string, int> animations => _animations.dictionary;

	//public float rangeRandomTime;

	public float minTimeAnimation;
	public float maxTimeAnimations;

	public string run;
	public string idle;

	public string targetAnimations;

	private float time;
	private float _timer;

    void Start()
    {

		armature = GetComponent<DragonBones.UnityArmatureComponent>();
		targetAnimations = idle;
		time = Random.Range(minTimeAnimation, maxTimeAnimations);
		armature.animation.FadeIn(idle);
    }

    // Update is called once per frame
    void Update()
    {

		if (targetAnimations == idle) {
			_timer += Time.deltaTime;
		} else {
			if (armature.animation.isCompleted) {
				targetAnimations = idle;
				armature.animation.Play(idle);
			}
		}
		if (_timer >= time) {
			targetAnimations = run;
			armature.animation.Play(run,1);
			_timer = 0;
			time = Random.Range(minTimeAnimation, maxTimeAnimations);
		}
	}

	void SetAnimation() {

	}
}
