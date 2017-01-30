using UnityEngine;
using System.Collections;

public class Light_Settings : MonoBehaviour {

    

    Light aLight;
    Light_Movement lMovement;

    CircleCollider2D collider2d;

    float increments = 6;

    const float maxZ = 190;
    const float minZ = 63;

    const float maxRange = 880;
    const float minRange = 300;

    // Use this for initialization
    void Start () {
        aLight = GetComponent<Light>();
        lMovement = GetComponent<Light_Movement>();
        collider2d = GetComponent<CircleCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
	 
	}

    public void increaseLight(int ammount)
    {
        lMovement.mouseZ = lMovement.mouseZ - (ammount * ((maxZ - minZ) / increments));
        lMovement.mouseZ = lMovement.mouseZ < -maxZ ? -maxZ : lMovement.mouseZ;

        aLight.range = aLight.range + (ammount * ((maxRange - minRange) / increments));
        aLight.range = aLight.range > maxRange ? maxRange : aLight.range;

        collider2d.radius = aLight.range / 5.5f;
        /*if (aLight.spotAngle + (ammount * 20) > 179)
        {
            aLight.spotAngle = 179;
            return;
        }
        aLight.spotAngle += ammount * 10;*/
    }

    public void decreaseLight(int ammount)
    {
        lMovement.mouseZ = lMovement.mouseZ + (ammount * ((maxZ - minZ) / increments));
        lMovement.mouseZ = lMovement.mouseZ > -minZ ? -minZ : lMovement.mouseZ;

        aLight.range = aLight.range + (ammount * ((maxRange - minRange) / increments));
        aLight.range = aLight.range > maxRange ? maxRange : aLight.range;

        collider2d.radius = aLight.range / 4.5f;
        /*if (aLight.spotAngle - (ammount * 20) < 70)
        {
            aLight.spotAngle = 70;
            return;
        }
        aLight.spotAngle -= ammount * 10;*/
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Enemy_Statue") && other.GetType() == typeof(BoxCollider2D))
        {
            other.GetComponent<Statue_Movement>().InLight(true);
        }
        else if (other.gameObject.name.StartsWith("Turtle"))
        {
            other.GetComponent<Turtle_Movement>().InLight(true);
            other.GetComponent<Turtle_Movement>().Flip();
        }
        else if (other.gameObject.name.StartsWith("Lock_LightSwitch"))
        {
            other.GetComponent<Lock_LightSwitch>().InLight(true);
        }
        else if (other.gameObject.name.StartsWith("LightWall"))
        {
            other.GetComponent<LightWall_Behaviour>().InLight(true);
        }
        else if (other.gameObject.name.StartsWith("LightPlatform"))
        {
            other.GetComponent<LightWall_Behaviour>().InLight(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Enemy_Statue") && other.GetType() == typeof(BoxCollider2D))
        {
            other.GetComponent<Statue_Movement>().InLight(false);
        }
        else if (other.gameObject.name.StartsWith("Turtle"))
        {
            other.GetComponent<Turtle_Movement>().InLight(false);
        }
        else if (other.gameObject.name.StartsWith("Lock_LightSwitch"))
        {
            other.GetComponent<Lock_LightSwitch>().InLight(false);
        }
        else if (other.gameObject.name.StartsWith("LightWall"))
        {
            other.GetComponent<LightWall_Behaviour>().InLight(false);
        }
        else if (other.gameObject.name.StartsWith("LightPlatform"))
        {
            other.GetComponent<LightWall_Behaviour>().InLight(false);
        }
    }
}
