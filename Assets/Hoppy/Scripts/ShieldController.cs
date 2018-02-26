using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Written By Abdalla Tawfik & Fehr Hassan


public class ShieldController : MonoBehaviour {

  #region Variables Declaration & Initialization
  // A reference to Shield Explosion.
    public GameObject shieldExplosion;

	#endregion

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            // Collision detected with a Shield Object.
            // Deactivate The Shield Object.
            col.gameObject.SetActive(false);
            // Call powerup handler
            enableShield();
        }
    }

  #region powerUps
  void enableShield()
  {
    // turns on powerUp

    // Turn on shield effect on player = blue glow
  }
  void useShield()
  {
    // Instantiate Gem Explosion in the same position of the picked Gem.
    GameObject shieldExplosionObject = Instantiate(shieldExplosion, new Vector3(transform.position.x, shieldExplosion.transform.position.y, transform.position.z), shieldExplosion.transform.rotation) as GameObject;

    // Destroy Gem Explosion Object.
    Destroy(shieldExplosionObject, 2f);
  }
  #endregion
}
