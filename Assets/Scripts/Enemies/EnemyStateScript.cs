//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
namespace EnemyLogic
{
   public abstract class EnemyState : MonoBehaviour{
        private GameObject enemyObject;
        private EnemyScript enemyScript;

        /*public EnemyState(GameObject enemyObject){
            this.enemyObject = enemyObject;
            this.enemyScript = enemyObject.GetComponent<EnemyScript>();
        }*/
        public void Move(){}
        public void NextState(){}
   }
}
