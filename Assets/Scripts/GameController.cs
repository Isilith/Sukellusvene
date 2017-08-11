using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public enum Interraction { Moving, Driving, Fixing, Depth }

public class GameController : MonoBehaviour
{

    public GameObject driverStation;
    public GameObject driverSpeedStation;
    public GameObject depthStation;

    private float speed;
    private float angle;
    private float depthAngle;

	public Slider slider;
	Text objectName;

    public float maxSpeed;
    public float speedChangeRate;
    public float turnRate;
    public float depthTurnRate;
    public GameObject minePrefab;
    public float mineSpawnDistance;
    public float mineSpawnInterwall = 5;
    private List<Mine> mines = new List<Mine>();
    public List<Damage> damageSpots = new List<Damage>();
    public List<Station> stations = new List<Station>();
    public GameObject objectiveArrow;
    public GameObject arrowHolder;

    public AudioSource alarm;
    public AudioSource boom;
    public AudioSource cliche;
    public AudioSource wail;

    bool interracting = false;
    Interraction state = Interraction.Moving;

    public FirstPersonController firstPersonController;
    public SubmarineDriver submarineDriver;

    private static GameController instance;
    public static GameController Instance
    {
        get { return instance; }
    }

    void Start()
    {
        if (instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

		GameObject temp = GameObject.Find("ObjectName");
		objectName = temp.GetComponent<Text>();

        Helper.SetButtonText(driverStation, angle.ToString());
        Helper.SetButtonText(driverSpeedStation, speed.ToString("F2"));
        Helper.SetButtonText(depthStation, depthAngle.ToString("F2"));
        StartCoroutine(SpawnMines());
        DealDamage();
        SetDestination();
        StartCoroutine(TrackObjective());
        StartCoroutine(RandomSounds());
    }

    private Station currentTarget;
    private void SetDestination()
    {
        var tempList = stations.FindAll(x => x.active == false);
        if (currentTarget != null)
        {
            currentTarget.Disable();
        }
        currentTarget = tempList[Random.Range(0, tempList.Count)];
        currentTarget.SetActive();
        
    }


    int points = 0;
    public GameObject scoreText;
    IEnumerator TrackObjective()
    {
        while (true)
        {
            if (Vector3.Distance(submarineDriver.transform.position, currentTarget.transform.position) < 30f)
            {
                points++;
                Helper.SetButtonText(scoreText, "Score:\n" + points);
                SetDestination();
            }
            objectiveArrow.transform.position = Vector3.MoveTowards(arrowHolder.transform.position, currentTarget.transform.position, 35);
            
            //objectiveArrow.transform.position = arrowHolder.GetComponent<Collider>().bounds.ClosestPoint(currentTarget.transform.position);
            objectiveArrow.transform.LookAt(currentTarget.transform.position);

            yield return null;
        }
    }

    void Update()
    {

        switch (state)
        {
            case Interraction.Moving:
                break;
            case Interraction.Driving:
                Drive();
                break;
            case Interraction.Fixing:
                if (repairing == false)
                {
                    StartCoroutine(Repair());
                }
                return;
                
                break;
            case Interraction.Depth:
                Depth();
                break;
        }

		string[] list = {"Steering", "Acceleration", "Damage", "DepthMeter", "DivingAngle"};
		RaycastHit hit2;
		var layerMask2 = 1 << 8;
		if (Physics.Raycast(firstPersonController.m_Camera.transform.position, firstPersonController.m_Camera.transform.forward, out hit2, 5, layerMask2))
		{
			for (int i=0; i<list.Length; i++)
				if (hit2.transform.parent.name.Contains(list[i])) {
					objectName.text = list[i];
					break;
				}
		}
		else
		{
			objectName.text = "";
		}


        if (Input.GetKeyDown(KeyCode.E))
        {
            if(interracting == true)
            {
                interracting = false;
                firstPersonController.enabled = true;
                state = Interraction.Moving;
                return;
            }
            Debug.DrawRay(firstPersonController.m_Camera.transform.position, firstPersonController.m_Camera.transform.forward, Color.cyan, 25);
            RaycastHit hit;
            var layerMask = 1 << 8;
            if (Physics.Raycast(firstPersonController.m_Camera.transform.position, firstPersonController.m_Camera.transform.forward, out hit, 5, layerMask))
            {
                switch (hit.transform.tag)
                {
                    case "DrivingWheel":
                        state = Interraction.Driving;
                        break;
                    case "DepthPanel":
                        state = Interraction.Depth;
                        break;
                    case "Damage":
                        state = Interraction.Fixing;
                        repairTarget = hit.transform.GetComponent<Damage>();
                        break;
                }
                firstPersonController.enabled = !firstPersonController.enabled;
                interracting = true;
            }
        }
    }

    private Damage repairTarget;
    private bool repairing = false;
    public float repairTime = 5f;
    IEnumerator Repair()
    {
		slider.gameObject.SetActive(true);
        float startTime = Time.time + repairTime;
        repairing = true;
        while (repairing)
        {
			if (slider != null)
				slider.value = (startTime-Time.time)/repairTime;
			
            if (startTime < Time.time)
            {
                repairTarget.Disable();
                damageamount--;
                repairing = false;
                interracting = false;
                firstPersonController.enabled = true;
                state = Interraction.Moving;
				slider.gameObject.SetActive(false);
				SetLightColor("Light1", new Color(0,1,0));
				SetLightColor("Light2", new Color(0,1,0));
            }
            yield return null;
        }
    }

	private static void SetLightColor(string name, Color col)
	{
		GameObject t = GameObject.Find(name);
		Light light = t.GetComponent<Light>();
		light.color = col;
	}



    private void Drive()
    {
        if (Input.GetKey(KeyCode.A))
        {
            angle -= turnRate * Time.deltaTime;
            if (angle < 0)
            {
                angle += 360;
            }
            submarineDriver.SetAngle(angle);
            var temp = Mathf.FloorToInt(angle);
            Helper.SetButtonText(driverStation, temp.ToString());
        }
        if (Input.GetKey(KeyCode.D))
        {
            angle += turnRate * Time.deltaTime;
            if (angle > 360)
            {
                angle -= 360;
            }
            submarineDriver.SetAngle(angle);
            var temp = Mathf.FloorToInt(angle);
            Helper.SetButtonText(driverStation, temp.ToString());
        }

        if (Input.GetKey(KeyCode.W))
        {
            speed += speedChangeRate * Time.deltaTime;
            speed = Mathf.Clamp(speed, -5, maxSpeed);
            Helper.SetButtonText(driverSpeedStation, speed.ToString("F2"));
            submarineDriver.SetMovementSpeed(speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            speed -= speedChangeRate * Time.deltaTime;
            speed = Mathf.Clamp(speed, -5, maxSpeed);
            Helper.SetButtonText(driverSpeedStation, speed.ToString("F2"));
            submarineDriver.SetMovementSpeed(speed);
        }
        

    }

    private void Depth()
    {
        if (Input.GetKey(KeyCode.W))
        {
            depthAngle += depthTurnRate * Time.deltaTime;
            submarineDriver.SetDepthAngle(depthAngle);
            depthAngle = Mathf.Clamp(depthAngle, -1, 1);
            Helper.SetButtonText(depthStation, depthAngle.ToString("F2"));
            
        }
        if (Input.GetKey(KeyCode.S))
        {
            depthAngle -= depthTurnRate * Time.deltaTime;
            submarineDriver.SetDepthAngle(depthAngle);
            depthAngle = Mathf.Clamp(depthAngle, -1, 1);
            Helper.SetButtonText(depthStation, depthAngle.ToString("F2"));
        }
        
    }

    public static void HitMine(Mine mine)
    {
        instance.boom.Play();
        instance.mines.RemoveAll(x => x.id == mine.id);

        DealDamage();
    }

    private IEnumerator RandomSounds()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            float endTime = Time.time + Random.Range(15, 30);
            int clip = Random.Range(0, 2);
            if (clip == 0)
            {
                wail.Play();
            }
            else
            {
                cliche.Play();
            }
            while (true)
            {
                if (endTime < Time.time)
                {
                    break;
                }
                yield return null;
            }
        }
    }

    int damageamount;
    public int damageMax;
    public static void DealDamage()
    {
		SetLightColor("Light1", new Color(1,0,0));
		SetLightColor("Light2", new Color(1,0,0));
        instance.alarm.Play();
        if (instance.damageamount > instance.damageMax)
        {
            //Gameover
            SceneManager.LoadScene(0);
        }
        else
        {
            var tempList = instance.damageSpots.FindAll(x => x.active == false);
            tempList[Random.Range(0, tempList.Count)].SetActive();
            instance.damageamount++;
        }
    }


    IEnumerator SpawnMines()
    {
        while (true)
        {
            /*
            for (int x = 0; x < mines.Count; x++)
            {
                if (mines[x] == null)
                {
                    mines.RemoveAt(x);
                    x--;
                }
            }
            */
            bool valid = true;
            int z = 0;
            Vector3 direction = Random.onUnitSphere;
            direction.y = 0; // limit to plane
            direction.Normalize();
            Mine mine = (Instantiate(minePrefab, submarineDriver.transform.position + direction * mineSpawnDistance, Quaternion.identity) as GameObject).GetComponent<Mine>();
            mine.id = Time.realtimeSinceStartup;

            while (mines.Exists(x => x == null? false : x.GetCollider().bounds.Intersects(mine.GetCollider().bounds)))
            {
                direction = Random.onUnitSphere;
                direction.y = 0; // limit to plane
                direction.Normalize();
                mine.transform.position = submarineDriver.transform.position + direction * mineSpawnDistance;
                if (z > 10)
                {
                    /*
                    Debug.LogError("LoopError");
                    mine.transform.position = new Vector3(100, 100, 100);
                    valid = false;
                    //Destroy(mine.transform.parent.gameObject);
                    */
                            
                    break;
                }
                yield return null;

                //z++;
            }

            if (valid == true)
            {
                mines.Add(mine);
            }
            yield return new WaitForSeconds(mineSpawnInterwall);
        }
        
    }

}
