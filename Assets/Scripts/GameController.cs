using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameController : MonoBehaviourPun
{
    [Header("Resources")]
    public int stock;
    public int scrolls;
    public int gold;

    [Header("UI Dev")]
    public Text stockTextDev;
    public Text scrollTextDev;
    public Text goldTextDev;
    public TextMeshProUGUI devModeText;
    public TextMeshProUGUI errorTextDev;
    public Text waveCountTextDev;
    public Text nextWaveTimeRemainingDev;


    [Header("UI Normal")]
    public Text stockTextNormal;
    public Text scrollTextNormal;
    public Text goldTextNormal;
    public TextMeshProUGUI errorTextNormal;
    public Text waveCountTextNormal;
    public Text nextWaveTimeRemainingNormal;
    public GameObject helpBar;
    public GameObject scoreBoard;
    public GameObject menu;
    public GameObject statusContainer;
    public TextMeshProUGUI statusText;
    public Button endedRestartButton;
    public GameObject endGameContainer;
    public TextMeshProUGUI player1FinalScoreText;
    public TextMeshProUGUI player2FinalScoreText;
    public TextMeshProUGUI resultText;




    

    [Header("Game Control")]
    public bool gameIsRunning;
    public bool mapReset;
    public bool ended;
    public bool canBuild;
    public bool devMode;
    public bool isCasting = false;
    public int unitsPerWave;
    public int maxWaveCount;
    public int waveTimer;
    public int unitMoveSpeed;
    public int dummyTimer;
    public int spellTimer;
    public float firstCountDown;
    public float lastCountDown;
    //public bool won;
    //public bool lost;
    public int player1FinalScore;
    public int player2FinalScore;
    public bool isMaster;
    public bool player1lost;
    public bool player2lost;
    


    [Header("Goal Controls")]
    public bool goalSpawned;
    public Vector3 goalPosition;
    public Vector2Int goalGridIndex;

    [Header("Manager Objects")]
    public UnitManager unitManager;
    public List<GameObject> spawnedObjects;
    public TowerManager towerManager;
    public GameObject devController;
    public GoalController goalController;
    

    [Header("Defense Game Objects (Prefab)")]
    public GameObject dmgTower;
    public GameObject roughTerrain;
    public GameObject wall;
    public GameObject goalObj;
    public GameObject gameWorld;

    [Header("Grid Management")]
    public GridController gridController;
    public Vector2Int gridIndex;
    public GridDebug gridDebug;

    [Header("Defense Costs")]
    public int towerCost;
    public int wallCost = 50;
    public int roughTerrainCost;

    public float timeToFadeText = 0;
    public float castDuration = 0;
    public float timeToResetGoal = 0;

    [Header("P1 Stats")]
    public TextMeshProUGUI P1_Stock;
    public TextMeshProUGUI P1_Scrolls;
    public TextMeshProUGUI P1_Gold;
    public TextMeshProUGUI P1_Turn;
    public TextMeshProUGUI P1_Units;

    [Header("P2 Stats")]
    public TextMeshProUGUI P2_Stock;
    public TextMeshProUGUI P2_Scrolls;
    public TextMeshProUGUI P2_Gold;
    public TextMeshProUGUI P2_Turn;
    public TextMeshProUGUI P2_Units;

    

    
    void Awake()
    {
        canBuild = true;
        gridController = GameObject.Find("GridController").GetComponent<GridController>();
    }

    void Start()
    {
        gridController.ControlGridCellCosts();
        spawnedObjects = new List<GameObject>();
        if(devMode){
            initDevMode();
        }
        else
        {
            initNormalMode();
        }

        
        endGameContainer.SetActive(false);
        statusContainer.SetActive(true);


        UpdateResources();
        UpdateWaveTexts();
        
        SetErrorText("");
        firstCountDown = 20;
        lastCountDown = 10;

        

    }

    public void UpdatePlayer1Stats(PlayerController player){
        P1_Stock.text = stock.ToString();
        P1_Scrolls.text = scrolls.ToString();
        P1_Gold.text = gold.ToString();
        P1_Turn.text = (unitManager.waveCount-1).ToString() + "/" + unitManager.maxWaveCount.ToString();
        P1_Units.text = unitManager.unitsInGame.Count.ToString();

        P2_Stock.text = player.enemyStock.ToString();
        P2_Scrolls.text = player.enemyScrolls.ToString();
        P2_Gold.text = player.enemyGold.ToString();
        P2_Turn.text = (player.enemyCurrentWave-1).ToString() + "/" + player.enemyMaxWaveCount.ToString();
        P2_Units.text = player.enemyUnitsAlive.ToString();
        
    }

    public void UpdatePlayer2Stats(PlayerController player){
        P2_Stock.text = stock.ToString();
        P2_Scrolls.text = scrolls.ToString();
        P2_Gold.text = gold.ToString();
        P2_Turn.text = (unitManager.waveCount-1).ToString() + "/" + unitManager.maxWaveCount.ToString();
        P2_Units.text = unitManager.unitsInGame.Count.ToString();

        P1_Stock.text = player.enemyStock.ToString();
        P1_Scrolls.text = player.enemyScrolls.ToString();
        P1_Gold.text = player.enemyGold.ToString();
        P1_Turn.text = (player.enemyCurrentWave-1).ToString() + "/" + player.enemyMaxWaveCount.ToString();
        P1_Units.text = player.enemyUnitsAlive.ToString();
    }

    


    void Update(){
        UpdateDevModeText();
        FadeText();
       
        CastEvent();
       UpdateWaveTexts();

       UpdateResources();

       unitManager.unitsPerWave = unitsPerWave;
       unitManager.maxWaveCount = maxWaveCount;
       unitManager.waveTimer = waveTimer;
       unitManager.unitMoveSpeed = unitMoveSpeed;

       if(Input.GetKeyDown(KeyCode.K))
       {
           //StartGame();
       }
       if(Input.GetKeyDown(KeyCode.L))
       {
           //ResetGame();
       }


       if(!gameIsRunning && !ended){
           if(firstCountDown > 0){
               statusText.text = "Építs védelmet! Kezdés: " + Mathf.Floor(firstCountDown).ToString() + " s";
               firstCountDown -= Time.deltaTime;
           }
           else{
               if(!gameIsRunning && !ended){
                   StartGame();
               }
           }
       }

       if(unitManager.waveCount == unitManager.maxWaveCount){
           if(lastCountDown > 0){
               statusText.text = "Hamarosan vége, tarts ki eddig: " + Mathf.Floor(firstCountDown).ToString() + " s";
               lastCountDown -= Time.deltaTime;
           }
           else{
               if(gameIsRunning){
                   ended = true;
               }
           }
       }
       else if(stock <= 0){
           stock = 0;
           if(gameIsRunning){
               ended = true;
           }
       }
       
    }


    public void UnderAttack(){
        SetErrorText("TÁMADÁS ALATT");
        unitManager.BoostUnits();
    }

    public void EndGame(){
        gameIsRunning = false;
        ended = true;
        goalController.CancelInvoke();
        unitManager.StopUnits();
        unitManager.canSpawn = false;
        ResetMap();
        unitManager.DestroyUnits();
        statusText.text = "Játék vége!";
        if(player1lost){
            player1FinalScore = 0;
        }else
        {
            player2FinalScore = 0;
        }
        player1FinalScoreText.text = "1. játékos: " + player1FinalScore.ToString() + " pont";
        player2FinalScoreText.text = "2. játékos: " + player2FinalScore.ToString() + " pont";



        if(isMaster){
            if(player1FinalScore > player2FinalScore){
                resultText.color = Color.green;
                resultText.text = "Gratulálunk! NYERTÉL!";
            }
            else{
                 resultText.color = Color.red;
                resultText.text = "Vesztettél. Talán legközelebb!";
            }
        }
        else{
            if(player1FinalScore < player2FinalScore){
                resultText.color = Color.green;
                resultText.text = "Gratulálunk! NYERTÉL!";
            }
            else{
                 resultText.color = Color.red;
                resultText.text = "Vesztettél. Talán legközelebb!";
            }
        }
        
        

        endGameContainer.SetActive(true);
        
    }


    public void UpdateWaveTexts(){
        if(devMode){
            if(gameIsRunning){
                waveCountTextDev.text = unitManager.waveCount-1 + " / " + maxWaveCount;
                nextWaveTimeRemainingDev.text = Mathf.Floor(unitManager.waveCountDown+1) + " s";
            }
            else
            {
                waveCountTextDev.text = 1 + " / " + maxWaveCount;
                nextWaveTimeRemainingDev.text = "...";
            }
        }
        else
        {
            if(gameIsRunning){
                waveCountTextNormal.text = unitManager.waveCount-1 + " / " + maxWaveCount;
                nextWaveTimeRemainingNormal.text = Mathf.Floor(unitManager.waveCountDown+1) + " s";
            }
            else
            {
                waveCountTextNormal.text = 1 + " / " + maxWaveCount;
                nextWaveTimeRemainingNormal.text = "...";
            }
        }
    }


    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void SetErrorText(string _errorText){
        if(devMode){
            errorTextDev.text = _errorText;
        }
        else
        {
            errorTextNormal.text = _errorText;
        }
        
        Timer("error",1.5f);
    }

    public void FadeText(){
        if(timeToFadeText > 0){
           timeToFadeText -= Time.deltaTime;
       }else{
           if(devMode){
               errorTextDev.text = "";
           }
           else{
               errorTextNormal.text = "";
           }
           
       }
    }

    public void CastSpell(bool dummy = false){
        if(isCasting){
            SetErrorText("Varázslás folyamatban. Várj ~" + Mathf.Floor(castDuration) + " másodpercet");
        }else{
            if(!dummy)
            {
                if(scrolls > 0)
                {
                    if(!devMode){
                        changeScrolls(-1);
                        scrollTextNormal.text = scrolls.ToString();
                    }
                    Timer("casting",spellTimer);
                }
                else{
                    SetErrorText("Nincs elég tekercs");
                }
            }
            else{
                if(stock > 10){
                    if(!devMode){
                        changeStock(-10);
                        stockTextNormal.text = stock.ToString();
                    }
                    Timer("casting",spellTimer);
                }
                else{
                    SetErrorText("Alacsony készlet!");
                }
            }
        }
    }


    
    public void CastEvent(){
        if(castDuration > 0)
        {
            isCasting = true;
            castDuration -= Time.deltaTime;
        }else{
            isCasting = false;
        }
    }

    public void Timer(string type,float seconds){
        switch(type){
            case "error":
                timeToFadeText = seconds;
                break;
            case "casting":
                castDuration = seconds;
                break;
        }
    }


    
    
    

    public void changeGold(int amount)
    {
        this.gold += amount;   
    }

    public void changeStock(int amount){
        this.stock += amount;   
    }

    public void changeScrolls(int amount){
        this.scrolls += amount;   
    }

    public void buildTower(Vector3 cellPosition)
    {   
        if(canBuild)
        {   
            TowerController towerController = dmgTower.GetComponent<TowerController>();
            bool canBuy = (this.gold - towerCost) >= 0;
            if(canBuy){
                //költség levonása
                this.changeGold(-towerCost);
                //játéktérbe teremtése - ez a CellManager-ben van meghívva, és az átadja a saját pozícióját
                GameObject newTower = Instantiate(dmgTower, cellPosition, Quaternion.identity);
                //a torony controller objektum eltárolja az összes létrehozott tornyot egy tömbben
                towerManager.addTower(newTower);
                spawnedObjects.Add(newTower);
                UpdateResources();
            }else{
                SetErrorText("Nincs elég arany");
            }
        }
        else{
            SetErrorText("Most nem tudsz építeni");
        }
    }

    public void buildWall(Vector3 cellPosition)
    {
        if(canBuild)
        {
            bool canBuy = (this.gold - wallCost) >= 0;
            if(canBuy)
            {
                
                this.changeGold(-wallCost);
                GameObject newWall = Instantiate(wall, cellPosition, Quaternion.identity);
                spawnedObjects.Add(newWall);
                UpdateResources();
            }
            else{
                 SetErrorText("Nincs elég arany");
            }
        }
        else
        {
             SetErrorText("Most nem tudsz építeni");
        }
    }

    public void buildTerrain(Vector3 cellPosition)
    {
        if(canBuild){
            bool canBuy = (this.gold - roughTerrainCost) >= 0;
            if(canBuy)
            {
                this.changeGold(-roughTerrainCost);
                GameObject newTerrain = Instantiate(roughTerrain, cellPosition, Quaternion.identity);
                spawnedObjects.Add(newTerrain);
                UpdateResources();
            }
            else
            {
                SetErrorText("Nincs elég arany");
            }
        }
        else
        {
            SetErrorText("Most nem tudsz építeni");
        }
        
    }

    

    

    //cell átadja a saját gridindexét
    public void DummyGoal(Vector3 cellPosition,Vector2Int gridIndex)
    {
        if(!canBuild){
                //Elindítja a DummyGoal nevű időzítéssel rendelkező metódust
                gridController.setDummyGoal(cellPosition,gridIndex);
        }
        else
        {
            //írjuk ki, hogy nem lehet építeni, sem törölgetni!!
            SetErrorText("Csak játék közben lehet csali célt elhelyezni!");
        }
    }

    public void UpdateDevModeText(){
        if(devMode){
            devModeText.text = "BE";
            devModeText.color = Color.green;
        }
        else{
             devModeText.text = "KI";
            devModeText.color = Color.red;
        }
       
        
    }

    public void TriggerDevMode()
    {
        if(devMode)
        {
            //kikapcs
            devMode = false;
            UpdateDevModeText();
            initNormalMode();
        }
        else
        {
            //bekapcs
            devMode = true;
            UpdateDevModeText();
            initDevMode();
        }
            
    }



    public void InitGame(){
        //FindGoal();
        unitManager.unitMoveSpeed = unitMoveSpeed;
        unitManager.unitsPerWave = unitsPerWave;
        unitManager.InitSpeed();
        SetErrorText("");
        endGameContainer.SetActive(false);
    }

    

    public void initDevMode()
    {
        towerCost = 0;
        wallCost = 0;
        roughTerrainCost = 0;
        devController.GetComponent<DevModeTools>().saveDevStats();
        ResetGoal(goalPosition);
        InitGame();
        gridController.dummyTimer = dummyTimer;
        gridController.ControlGridCellCosts();
        unitManager.canSpawn = false;
        canBuild = true;
        firstCountDown = 0;
        gameIsRunning = false;
        
    }

    public void initNormalMode()
    {
        towerCost = 100;
        wallCost = 50;
        roughTerrainCost = 30;
        stock = 100;
        scrolls = 5;
        gold = 350;
        goalPosition = InitializeGoalPosition();
        ResetGoal(goalPosition);
        InitGame();
        gridController.SetGridCellCosts(3);

        unitsPerWave = 10;
        maxWaveCount = 2;
        unitMoveSpeed = 3;
        player1lost = false;
        player2lost = false;
        firstCountDown = 20;
        lastCountDown = 5;
        statusContainer.SetActive(true);
        endGameContainer.SetActive(false);
        ended = false;
        
    }



    public void StartGame()
    {
        ended = false;
        gameIsRunning = true;
        //player.setObjectsActive();
        unitManager.BeginSpawning();
        gridController.setGoal(goalPosition);
        gridController.disableBuild();
        canBuild = false;
        statusContainer.SetActive(false);
        endGameContainer.SetActive(false);
        
    }

    [PunRPC]
    public void ResetGame(){
        ended = true;
        gameIsRunning = false;
        gridController.enableBuild();        
        gridDebug.ClearCellDisplay();

        unitManager.InitUnitStats();
        towerManager.InitTowers();

        canBuild = true;


        if(devMode){
            initDevMode();
        }else{
            initNormalMode();
        }
        
        
    }

    public void OnResetAllGames(){
        photonView.RPC("ResetGame",RpcTarget.All);
    }

    public void UpdateResources(){
        if(devMode){
            stockTextDev.text = stock.ToString();
            scrollTextDev.text = scrolls.ToString();
            goldTextDev.text = gold.ToString();
        }
        else{
            stockTextNormal.text = stock.ToString();
            scrollTextNormal.text = scrolls.ToString();
            goldTextNormal.text = gold.ToString();
        }
        
    }

    // megkeresi a goal objectet, ha megvan, akkor visszatér vele. Ha nem, akkor a manage script  lefut goalspawned = false paraméterrel és létrehozza
    public GameObject FindGoalObject(){
        
        GameObject goal = GameObject.FindWithTag("goal");
        Debug.Log(goal);
        if(goal == null){
            Debug.Log("nincs cél");
        }
        return goal;
    }

    public void ResetGoal(Vector3 _pos){
        if(_pos == new Vector3(0,0,0)){
            _pos = InitializeGoalPosition();
        }
        if(goalSpawned){
            GameObject goalToBeDestroyed = FindGoalObject();
            Destroy(goalToBeDestroyed);

            goalPosition = InitializeGoalPosition();
            buildGoal(_pos);
        }
        else
        {
            goalPosition = InitializeGoalPosition();
            buildGoal(_pos);
        }

    }

    //csinál egy random goal positiont és visszatér vele
    public Vector3 InitializeGoalPosition(){
            float randX = Mathf.Floor(Random.Range(45.0f,50.0f));
            float randY = Mathf.Floor(Random.Range(1.0f,25.0f)); 
            goalPosition = new Vector3(randX+0.5f,0,randY+0.5f);
            return goalPosition;
	}



    public void buildGoal(Vector3 cellPosition){
            Instantiate(goalObj, cellPosition, Quaternion.identity);   
            goalSpawned = true; 
            goalPosition = cellPosition;
            goalGridIndex.x = (int)(cellPosition.x + 0.5f);
            goalGridIndex.y = (int)(cellPosition.z + 0.5f);
            gridController.setGoal(cellPosition);
            gridController.ControlGridCellCosts(); 
              
    }
    
    public void ResetMap()
    {
        if(!gameIsRunning && !mapReset){
            foreach (GameObject obj in spawnedObjects)
            {
                Destroy(obj);
                Debug.Log(obj + " is destroyed");
            }
            GameObject existingWorld = GameObject.Find("GameWorld");
            Destroy(existingWorld);
            Instantiate(gameWorld,new Vector3(0,0,0),Quaternion.identity);
            towerManager.towersInGame = new List<GameObject>();
            mapReset = true;
        }
    }

    public void showHelp(){
         if(helpBar.activeSelf){
            helpBar.SetActive(false);
        }
        else
        {
            helpBar.SetActive(true);
        }
    }

    public void showScoreBoard(){
         if(scoreBoard.activeSelf){
            scoreBoard.SetActive(false);
        }
        else
        {
            scoreBoard.SetActive(true);
        }
    }


    public void showOrHideMenu(){
        if(menu.activeSelf){
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
    }

    public void ToggleIcons(){
        if( gridDebug.curDisplayType == FlowFieldDisplayType.OnlyDestination)
        {
             gridDebug.curDisplayType = FlowFieldDisplayType.AllIcons;
        }
        else
        {
             gridDebug.curDisplayType = FlowFieldDisplayType.OnlyDestination;
        }
    }

    



    

}
