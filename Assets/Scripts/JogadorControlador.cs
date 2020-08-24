using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JogadorControlador : MonoBehaviour
{

    // half of the size of the cube
    const float CubeRadius = 0.5f;

    // how long to flip 90 degrees?
    const float TimeRequiredToFlip90 = 0.5f;

    public enum Direction
    {
        NONE = -1,
        FORWARD = 0,
        RIGHT,
        BACK,
        LEFT,
    };

    Direction Command;

    Direction CurrentRotation;
    float NonzeroWhenMoving;

    void GetInput()
    {
        Command = Direction.NONE;

        if (Input.GetAxis("Horizontal") < -0.05f) Command = Direction.LEFT;
        if (Input.GetAxis("Horizontal") > 0.05f) Command = Direction.RIGHT;
        if (Input.GetAxis("Vertical") < -0.05f) Command = Direction.BACK;
        if (Input.GetAxis("Vertical") > 0.05f) Command = Direction.FORWARD;
    }

    void Start()
    {
        NonzeroWhenMoving = 0;
        Pivoter = new GameObject("Pivoter");
    }

    GameObject Pivoter;

    void HandleMovingAndStopping()
    {
        float fraction = NonzeroWhenMoving / TimeRequiredToFlip90;

        NonzeroWhenMoving += Time.deltaTime;

        if (fraction >= 1)
        {
            fraction = 1;    // end precisely!

            NonzeroWhenMoving = 0.0f;    // done moving
        }

        float angle = 90 * fraction;

        switch (CurrentRotation)
        {
            case Direction.FORWARD:
                Pivoter.transform.rotation = Quaternion.Euler(angle, 0, 0);
                break;
            case Direction.BACK:
                Pivoter.transform.rotation = Quaternion.Euler(-angle, 0, 0);
                break;
            case Direction.LEFT:
                Pivoter.transform.rotation = Quaternion.Euler(0, 0, angle);
                break;
            case Direction.RIGHT:
                Pivoter.transform.rotation = Quaternion.Euler(0, 0, -angle);
                break;
        }

        if (NonzeroWhenMoving == 0)
        {
            transform.SetParent(null);    // deparent us from the pivoter
        }
    }

    void Update()
    {
        // we always GET the command, but we might not process it.
        // We only process commands when we reach flatness again.
        GetInput();

        if (NonzeroWhenMoving > 0)
        {
            HandleMovingAndStopping();
        }

        if (NonzeroWhenMoving == 0)
        {
            if (Command != Direction.NONE)
            {
                Vector3 SpotBeneathUs = transform.position + Vector3.down * CubeRadius;

                Vector3 WhereToPivot = SpotBeneathUs;

                switch (Command)
                {
                    case Direction.FORWARD:
                        WhereToPivot += Vector3.forward * CubeRadius;
                        break;
                    case Direction.BACK:
                        WhereToPivot += Vector3.back * CubeRadius;
                        break;
                    case Direction.LEFT:
                        WhereToPivot += Vector3.left * CubeRadius;
                        break;
                    case Direction.RIGHT:
                        WhereToPivot += Vector3.right * CubeRadius;
                        break;
                }

                CurrentRotation = Command;

                // move the pivoter to where it needs to be
                Pivoter.transform.position = WhereToPivot;
                Pivoter.transform.rotation = Quaternion.identity;

                // parent us onto it so we flip over
                transform.SetParent(Pivoter.transform);

                NonzeroWhenMoving += Time.deltaTime;    // trigger motion

                HandleMovingAndStopping();
            }
        }
    }
}
