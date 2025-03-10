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
    public class State : EnemyState{
        // Atributos privados de inspector

        // Atributos privados
        private GameObject enemyObject;
        private EnemyScript enemyScript;

        State(GameObject enemyObject){
            this.enemyObject = enemyObject;
            this.enemyScript = enemyObject.GetComponent<EnemyScript>();
        }

        public void Move(){
        // Se ejecuta en el FixedUpdate(), a 100fps

        }
        public void NextState(){
        // Define las condiciones para pasar al siguiente estado
        }
    }
}
