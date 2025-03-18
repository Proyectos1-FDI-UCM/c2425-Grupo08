//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------
using UnityEngine;

namespace PlayerLogic
{

    public abstract class PlayerState: MonoBehaviour{
        //private PlayerScript player;
        private GameObject playerObject ;
        public abstract void Move();
        public abstract void NextState();
        public abstract void SetPlayer(GameObject player);

    }
}
