//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
namespace EnemyLogic
{
   abstract class EnemyState{
        GameObject enemy;

        public void Move(){}
        public void NextState(){}
   }
}
