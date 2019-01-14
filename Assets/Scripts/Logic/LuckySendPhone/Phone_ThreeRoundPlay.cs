using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Phone_ThreeRoundPlay : ThreeRoundPlay {

    public Phone_ThreeRoundPlay(GameCtr _sdk) : base(_sdk)
    {

    }

    public override void StartGame(GameObject police, Transform catchMove)
    {
        KillTween();
        NormalPaly(police, catchMove);
        if (playAction != null)
            playAction();
        
    }

}
