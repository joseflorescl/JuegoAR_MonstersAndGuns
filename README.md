# JuegoAR_MonstersAndGuns
 Proyecto AR - Curso Unity Núcleo Escuela

Monsters 'n Guns<br>
Clon de Ghosts 'n Guns AR - Unity

Juego en Realidad Aumentada - Trabajo Final - Núcleo Escuela - Unity<br>
Este trabajo es un clon del juego para dispositivos móviles Ghosts 'n Guns.<br>
Es un juego de Realidad Aumentada, tipo shooter en primera persona, en donde el jugador tiene que defenderse de un ejército de fantasmas pixelados, que invaden el mundo.<br>

Programado por: José Leonardo Flores<br>
Arte y sonido: descargado de Internet<br>

  - El jugador, usando la realidad aumentada, debe ubicar en el espacio real un portal desde donde comenzarán a salir diversos tipos de enemigos que se moverán en el entorno.
  - El jugador puede disparar con 2 armas hacia el centro de la pantalla, con un solo touch. Si se mantiene presionado el dedo en la pantalla se realizan disparos en ráfaga.
  - En la UI se muestra un mini mapa o radar indicando la posición de los monsters con respecto al player.
  - Una vez que el jugador elimine a estos enemigos iniciales aparecerá el jefe final del nivel, el cual es más difícil de eliminar por tener un movimiento más complicado y porque dispara misiles.
  - Los diversos enemigos comparten el mismo script de AI, pero la data de configuración de su Scriptable Object es diferente.
  - Hay un enemigo jefe, con su propio script de AI.
  - El juego usa 4 escenas que se cargan de forma aditiva asíncronamente.
  - Managers: Game Manager, Scene Controller, Audio Manager, Spawner Manager, Battle Manager, VFX Manager, Vibration Manager, Pool Manager.
  - Para la librería de AR se usa AR Foundation - AR Core.
  - Código fuente: https://github.com/joseflorescl/JuegoAR_MonstersAndGuns
  - Puedes descargar el apk en: https://joseleonardo.itch.io/monsters-n-guns