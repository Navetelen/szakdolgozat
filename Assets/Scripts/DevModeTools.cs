using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class DevModeTools : MonoBehaviourPun
{
    public Player player;
    public GameController gameController;
    public GameObject devTools;
    public InputField dev_playerStockValue,dev_playerScrollValue,dev_playerGoldValue,dev_unitsSpawnedPerWaveValue,dev_maxWaveCountValue,dev_waveTimerValue,dev_unitMovementSpeedValue,dev_dummyGoalTimerValue,dev_spellTimerValue;
    public TMP_Dropdown dev_iconType;

    public FlowFieldDisplayType curDisplayType;

    public GridDebug gridDebug; 

    void Awake(){
        if(PhotonNetwork.IsMasterClient)
            player = PhotonNetwork.LocalPlayer;

        curDisplayType = gridDebug.curDisplayType;
    }

    void Start(){
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        initDevStats();

        gridDebug = GameObject.Find("GridDebug").GetComponent<GridDebug>();
    }

    void Update(){
        
    }
    
    public void initDevStats(){
        //showOrHideDevTools();
        dev_playerStockValue.text = gameController.stock.ToString();
        dev_playerScrollValue.text = gameController.scrolls.ToString();
        dev_playerGoldValue.text = gameController.gold.ToString();

        dev_unitsSpawnedPerWaveValue.text = gameController.unitsPerWave.ToString();
        dev_maxWaveCountValue.text = gameController.maxWaveCount.ToString();
        dev_waveTimerValue.text = gameController.waveTimer.ToString();
        dev_unitMovementSpeedValue.text = gameController.unitMoveSpeed.ToString();
        dev_dummyGoalTimerValue.text = gameController.dummyTimer.ToString();
        dev_spellTimerValue.text = gameController.spellTimer.ToString();
        switch(curDisplayType){
            case FlowFieldDisplayType.None:
                dev_iconType.value = 0;
                break;
            case FlowFieldDisplayType.AllIcons:
                dev_iconType.value = 1;
                break;
            case FlowFieldDisplayType.OnlyDestination:
                dev_iconType.value = 2;
                break;
            case FlowFieldDisplayType.CostField:
                dev_iconType.value = 3;
                break;
            case FlowFieldDisplayType.IntegrationField:
                dev_iconType.value = 4;
                break;
        }
    
    }

    public void saveDevStats(){
        gameController.stock = int.Parse(dev_playerStockValue.text);
        gameController.scrolls = int.Parse(dev_playerScrollValue.text);
        gameController.gold = int.Parse(dev_playerGoldValue.text);
        gameController.unitsPerWave = int.Parse(dev_unitsSpawnedPerWaveValue.text);
        gameController.maxWaveCount = int.Parse(dev_maxWaveCountValue.text);
        gameController.waveTimer = int.Parse(dev_waveTimerValue.text);
        gameController.unitMoveSpeed = int.Parse(dev_unitMovementSpeedValue.text);
        gameController.dummyTimer = int.Parse(dev_dummyGoalTimerValue.text);
        gameController.spellTimer = int.Parse(dev_spellTimerValue.text);
        switch(dev_iconType.value){
            case 0: 
                gridDebug.curDisplayType = FlowFieldDisplayType.None;
                break;
            case 1: 
                gridDebug.curDisplayType = FlowFieldDisplayType.AllIcons;
                break;
            case 2: 
                gridDebug.curDisplayType = FlowFieldDisplayType.OnlyDestination;
                break;
            case 3: 
                gridDebug.curDisplayType = FlowFieldDisplayType.CostField;
                break;
            case 4: 
                gridDebug.curDisplayType = FlowFieldDisplayType.IntegrationField;
                break;
                
        }

        ClampDevStats();

        gameController.InitGame();

        //showOrHideDevTools();
        //resetGame();
    }

    public void FindGOs(){
        devTools = GameObject.FindWithTag("devTools");
        showOrHideDevTools();
    }

    public void ClampDevStats(){
        //CLAMP
        ClampValues(dev_playerStockValue,1,1000);
        ClampValues(dev_playerScrollValue,1,1000);
        ClampValues(dev_playerGoldValue,1,100000);
        ClampValues(dev_unitsSpawnedPerWaveValue,1,200);
        ClampValues(dev_maxWaveCountValue,1,100);
        ClampValues(dev_waveTimerValue,1,100);
        ClampValues(dev_unitMovementSpeedValue,1,50);
        ClampValues(dev_dummyGoalTimerValue,1,100);
        ClampValues(dev_spellTimerValue,1,100);
    }

    void ClampValues(InputField inputField, int minVal, int maxVal){
        if(int.Parse(inputField.text) < 0) inputField.text = Mathf.Abs(float.Parse(inputField.text)).ToString();
        if(int.Parse(inputField.text) == 0) inputField.text = minVal.ToString();
        if(int.Parse(inputField.text) > 100) inputField.text = maxVal.ToString();
    }


    public void showOrHideDevTools(){
        if(devTools.activeSelf){
            devTools.SetActive(false);
        }
        else
        {
            devTools.SetActive(true);
        }
    }


}
