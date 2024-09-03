using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pellet : MonoBehaviour
{
   public int points = 10;

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
         Eat();
      }
   }
   protected virtual void Eat()
   {
      GameManager.Instance.PelletEaten(this);
   }

}
