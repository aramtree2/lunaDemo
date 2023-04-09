using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmController : MonoBehaviour
{
    public float BPM; // 1분동안 1박이 몇번 반복되는지
    public float Meter; // 1 마디동안 몇개의 박이 있는지
    public GameObject[] blockBeatType; // 각 박마다 어떤 블럭이 켜지고 꺼질지 설정함
    public AudioClip[] AudioClips; // 리듬의 리스트
    public int[] AudioClipsSplit; // 구간별 마지막 인덱스
    private AudioSource audioSource;
    public float beatTime; // 몇박 지났는지 계산용
    public int currentBeat; // 몇박 지났는지 계산용
    private bool changed; // 박자 넘어갔을때 블럭 바꿨는지 계산용
    public int AudioClipsType;
    public Transform position;
    public int phase;

    void PlayRandom(AudioClip[] clips, AudioSource audio, int[] clipsIndex, int phase)
		{
			if (clips != null && clips.Length > 0)
			{
                int index;
                if(phase == 0){
                    index = 0;
                }
                else{
                    index = Random.Range(clipsIndex[phase-1]+1, clipsIndex[phase]);
                }
				if (clips[index])
				{
					audio.Stop();
					audio.PlayOneShot(clips[index]);
				}
			}
		}

    void BlockChange(int currentBeat){
        if(!changed){
            if(currentBeat == 0){ // 0이면 마지막꺼랑 비교
                if(blockBeatType[0] == blockBeatType[blockBeatType.Length - 1]){ // 마지막 박자랑 같으므로 변화없이 넘어감
                    return;
                }
                else{ // 다르니까 바꿈
                    blockBeatType[0].SetActive(true);
                    blockBeatType[blockBeatType.Length - 1].SetActive(false);
                }
            }else{
                if(blockBeatType[currentBeat] == blockBeatType[currentBeat - 1]){ // 마지막 박자랑 같으므로 변화없이 넘어감
                    return;
                }
                else{ // 다르니까 바꿈
                    blockBeatType[currentBeat].SetActive(true);
                    blockBeatType[currentBeat - 1].SetActive(false);
                }
            }
            changed = true;
        }else{
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        beatTime = 0f;

        for(int i=1;i<Meter;i++){
            blockBeatType[i].SetActive(false);
        }

        PlayRandom(AudioClips, audioSource, AudioClipsSplit, 0);
        phase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 박자 세기
        beatTime += Time.deltaTime*BPM/60f;
        if(beatTime >= 1){
            beatTime -= 1;
            currentBeat++;
            changed = false;
        }
        if(currentBeat == Meter){
            currentBeat = 0;
            PlayRandom(AudioClips, audioSource, AudioClipsSplit, phase);
        }
        BlockChange(currentBeat);

        if(position.position.z < 2){
            phase = 0;
        }else if(position.position.z < 55){
            phase = 1;
        }else if(position.position.z < 184){
            phase = 2;
        }else if(position.position.z < 444){
            phase = 3;
        }else{
            phase = 4;
        }
    }
}
