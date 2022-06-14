using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Az egyes teremtett torony objektumokra rakott szkript
public class TowerController : MonoBehaviour
{
    public GameObject manaBar;
    public float mana;
    Renderer manaBarRenderer;
    public float manaWidth;
    public Vector3 scaleChange;

    public GameController gameController;



    GameObject Tower;
    public GameObject dangerZone;
    bool isDamaging;

    CapsuleCollider myCollider;

    public List<GameObject> unitsNearby;

    

    // Start is called before the first frame update
    void Start()
    {
        //A toronyhoz van csatolva egy ütköző, ami nagyobb mint a torony (felülről ezt látjuk annak a halvány körnek, mert a megjlenítés kedvéért egy materialt is kapott)
        myCollider = GetComponent<CapsuleCollider>();
        mana = 0;
        isDamaging = false;
        //Van egy energiát jelző csík is a torony felett
        manaBarRenderer = manaBar.GetComponent<Renderer>();
        manaBarRenderer.material.SetColor("_Color", Color.red);

        manaBar.transform.localScale = new Vector3(0,0.15f,0);



        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        InvokeRepeating("GatherMana",1f,1f);
    }

    void FixedUpdate()
    {
        //A hatókör növelést azonnal elsütő képesség elindítása az "A" billentyűre
        

        if(mana< 0){
            this.setMana(0);
        }

        if(mana > 100){
            this.setMana(100);
        }
        //Energia jelző csík színezése
        switch(mana){
            case 100:
                //Ha az energia eléri a 100-at használjuk a képességet (hatótáv növekedés)
                StartCoroutine("UseSpell");
                manaBarRenderer.material.SetColor("_Color", Color.green);
                break;
            case 75:
                manaBarRenderer.material.SetColor("_Color", Color.white);
                break;
            case 50:
                manaBarRenderer.material.SetColor("_Color", Color.yellow);
                break;
            case 25:
                manaBarRenderer.material.SetColor("_Color", Color.magenta);
                break;
            case 0:
                manaBarRenderer.material.SetColor("_Color", Color.red);
                break;
        }   

        //Mana alapján az energia jelző csík szélességének állítása
        manaWidth = (float)this.getMana()/50;
        scaleChange = new Vector3(manaWidth,0.15f,0);
        manaBar.transform.localScale = scaleChange;
    }

    public void OnTriggerEnter(Collider other){
        //Ha az ütközőn belül egy Unit tartózkodik, az energia folyamatosan növekedik
		if (other.gameObject.CompareTag("Unit"))
        {

            isDamaging = true;
            unitsNearby.Add(other.gameObject);
 
        }
    }
    
    
    void OnTriggerExit(Collider other){
            isDamaging = false;
            unitsNearby.Remove(other.gameObject);

    }


    //Energia generálás ha éppen sebez
    public void GatherMana(){
        if(isDamaging){
            this.setMana((this.getMana() + unitsNearby.Count));
            foreach(GameObject go in unitsNearby){
                go.GetComponent<UnitController>().TakeDamage(25);
            }
        }
        
    }

    //Hatókör megnövelése, majd 3 másodperc után annak visszaállítása, és az energia visszaállítása nullára
    IEnumerator UseSpell(){
        
        myCollider.radius = 5;
        dangerZone.transform.localScale  = new Vector3(10,10,10);
        yield return new WaitForSeconds(3f);
        dangerZone.transform.localScale  = new Vector3(6,6,6);
        myCollider.radius = 3;
        setMana(0);

    }

    public float getMana(){
        return this.mana;
    }

    public void setMana(float amount){
        this.mana = amount;
    }

}
