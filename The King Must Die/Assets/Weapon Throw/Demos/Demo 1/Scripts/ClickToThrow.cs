using UnityEngine;

public class ClickToThrow : MonoBehaviour
{
    public WeaponThrow script;

    // Update is called once per frame
    void Update()
    {   
        //left mouse click to throw
        if (Input.GetMouseButtonDown(0)) {
            //trigger the throw method only IF NOT ALREADY THROW (isThrown == false)
            if(!script.isThrown){
                script.Throw();
            }
        }

        //right mouse click to return weapon
        if (Input.GetMouseButtonDown(1)) {
            //trigger the return weapon method only IF WEAPON IS THROWN (isThrow == true)
            if (script.isThrown) {
                script.ReturnWeapon();
            }
        }
    }
}
