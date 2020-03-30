using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;


public class RhythmInput
{
    public KeyCode inputKey;
    public double inputTime;

    //only relevant for updatedInputSystem
    public string inputString;
}

/// <summary>
/// The purpose of this class is twofold:
/// - Get the Clock-synchronized timing of the user's input
/// - Check that against the windows of currently existing obstacles
/// </summary>
public class InputEvaluator : MonoBehaviour
{
    
    public List<FallingGem> activeGems;
    public List<RhythmInput> CachedInputs = new List<RhythmInput>();

    public Beatmap currentBeatmap;

    //Updated Input system uses Unity's Input manager
    public bool usingUnityInputManager;

    //ideally we'd manage score on a seperate script
    public int gameScore;

 
    void Awake()
    {
        usingUnityInputManager = currentBeatmap.useFallingGemInputClass;
    }


    void Update()
    {

        if(usingUnityInputManager)
        {
            for (int i = 0; i < currentBeatmap.fallingGemInputs.Length; i++)
            {

                if (Input.GetButtonDown(currentBeatmap.fallingGemInputs[i].playerInput))
                {
                    RhythmInput _rhythmInput = new RhythmInput();
                    _rhythmInput.inputString = currentBeatmap.fallingGemInputs[i].playerInput;
                    _rhythmInput.inputTime = Clock.Instance.TimeMS;

                    CachedInputs.Add(_rhythmInput);
                }
            }

        }
        else
        {
            for (int i = 0; i < currentBeatmap.playerInputKeys.Length; i++)
            {
                if (Input.GetKeyDown(currentBeatmap.playerInputKeys[i]))
                {
                    RhythmInput _rhythmInput = new RhythmInput();
                    _rhythmInput.inputKey = currentBeatmap.playerInputKeys[i];
                    _rhythmInput.inputTime = Clock.Instance.TimeMS;

                    CachedInputs.Add(_rhythmInput);
                }
            }
        }

        
        //old input system - check for keyboard inputs and log them
        



        //compare inputs to current beatMap windows

        //first find any non-destroyed cues

        FallingGem[] allGems = FindObjectsOfType<FallingGem>();

        activeGems.AddRange(allGems);
        for (int i = 0; i < activeGems.Count; i ++)
        {
            //we're not going to do anything with early inputs
            if (activeGems[i].gemCueState != FallingGem.CueState.Early)
            {
                //if player hasn't input anything, don't do anything
                if (CachedInputs.Count == 0)
                    break;
                //go through each of our inputs from this frame, and check them against this gem
                for (int j = 0; j < CachedInputs.Count; j++)
                {
                    if (CachedInputs[j].inputKey == activeGems[i].bmEvent.inputKey
                        || CachedInputs[j].inputString == activeGems[i].bmEvent.unityInput)
                    {
                        ScoreGem(activeGems[i]);
                        
                    }
                }
            }
        }

        //clear Lists
        activeGems.Clear();
        CachedInputs.Clear();

        

        
    }

    void ScoreGem(FallingGem gem)
    {
        switch (gem.gemCueState)
        {
            case FallingGem.CueState.OK:
                gameScore += 1;
                Debug.Log("OK!");
                Destroy(gem.gameObject);
                break;
            case FallingGem.CueState.Good:
                gameScore += 2;
                Debug.Log("Good!");
                Destroy(gem.gameObject);
                break;
            case FallingGem.CueState.Perfect:
                gameScore += 3;
                Debug.Log("Perfect!");
                Destroy(gem.gameObject);
                break;
            case FallingGem.CueState.Late:
                Debug.Log("Missed!");
                break;
        }


    }





}
