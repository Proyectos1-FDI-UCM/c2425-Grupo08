//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using EnemyLogic;
using UnityEngine;

namespace EnemyLogic
{
    class TemplateState : EnemyState{
        // Atributos privados de inspector

        // Atributos privados
        private GameObject enemyObject;
        private EnemyScript enemyScript;

        public TemplateState(GameObject enemyObject){
            this.enemyObject = enemyObject;
            this.enemyScript = enemyObject.GetComponent<EnemyScript>();
            Debug.Log("State: Template");
        }
        override public void Move(){
        // Se ejecuta en el FixedUpdate(), a 100fps
            Debug.Log("Moving Template");
        }
        override public void NextState(){
        // Define las condiciones para pasar al siguiente estado
        }
    }
}
