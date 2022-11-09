using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
    {
        right,
        left,
        down,
        up
    };

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField ] private Game _game;
    private bool isMoved = false;
    private Vector2 _touchStartPosition = Vector2.zero;
    private const float MinSwipeDistance = 10.0f;
    
  
    void Start()
    {

    }

    void Update()
    {
        #region Unity
            #if UNITY_EDITOR
                //if(Input.GetKeyDown(KeyCode.Right) && !isMoved)
            
                    if(Input.GetKeyDown(KeyCode.RightArrow) && !isMoved)
                    {   
                        isMoved = true;
                        _game.ShiftControl(Directions.right,0,1,1);
                        _gameManager.GameLoseCheck();
                        isMoved = false;
                    }
                    else if(Input.GetKeyDown(KeyCode.LeftArrow) && !isMoved)
                    {
                        isMoved = true;
                        _game.ShiftControl(Directions.left,0,-1,-1);
                        _gameManager.GameLoseCheck();
                         isMoved = false;
                    }
                    else if(Input.GetKeyDown(KeyCode.DownArrow) && !isMoved)
                    {
                         isMoved = false;
                        _game.ShiftControl(Directions.down,1,0,4);
                        _gameManager.GameLoseCheck();
                         isMoved = false;
                    }
                    else if(Input.GetKeyDown(KeyCode.UpArrow) && !isMoved)
                    {
                         isMoved = false;
                        _game.ShiftControl(Directions.up,-1,0,-4);
                        _gameManager.GameLoseCheck();
                         isMoved = false;
                    }
             
            #endif
        #endregion
    
        #region Android
            #if UNITY_ANDROID
                if (Input.touchCount == 0) 
                {
                    return;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Began) 
                {
                    _touchStartPosition = Input.GetTouch(0).position;
                }

                if (Input.GetTouch(0).phase != TouchPhase.Ended) 
                {
                    return;
                }

                var swipeDelta = (Input.GetTouch(0).position - _touchStartPosition);

                if (swipeDelta.magnitude < MinSwipeDistance) 
                {
                    return;
                }

                swipeDelta.Normalize();
            
                if(!isMoved)
                {
                    isMoved = true;
                    if (swipeDelta.y > 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f) {
                        _game.ShiftControl(Directions.up,-1,0,-4);
                        _gameManager.GameLoseCheck();
                    }
                    else if (swipeDelta.y < 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f) {
                        _game.ShiftControl(Directions.down,1,0,4);
                        _gameManager.GameLoseCheck();
                    }
                    else if (swipeDelta.x > 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f) {   
                        _game.ShiftControl(Directions.right,0,1,1);
                        _gameManager.GameLoseCheck();
                    }
                    else if (swipeDelta.x < 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f) {
                        _game.ShiftControl(Directions.left,0,-1,-1);
                        _gameManager.GameLoseCheck();
                    }
                    isMoved = false;
                }
            #endif
        #endregion
    }
}
