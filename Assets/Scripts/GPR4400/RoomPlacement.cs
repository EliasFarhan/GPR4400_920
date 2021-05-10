using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomPlacement : MonoBehaviour
{
    class Room
    {
        
    }
    private List<Rect> rooms = new List<Rect>();

    [SerializeField] private int roomNmb = 10;

    [SerializeField] private float minRoomSize = 1.0f;
    [SerializeField] private float maxRoomSize = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        var mainCamera = Camera.main;
        var cameraSize = 2.0f * mainCamera.orthographicSize * new Vector2(mainCamera.aspect, 1.0f);
        var cameraRect = new Rect(){min=-cameraSize/2.0f, max = cameraSize/2.0f};

        while (rooms.Count < roomNmb)
        {
            var size = new Vector2(Random.Range(minRoomSize, maxRoomSize), Random.Range(minRoomSize, maxRoomSize));
            var position = new Vector2(Random.Range(cameraRect.xMin, cameraRect.xMax - size.x),
                Random.Range(cameraRect.yMin, cameraRect.yMax-size.y));
            Rect newRoom = new Rect(position, size);
            bool contact = false;
            foreach (var room in rooms)
            {
                if (room.Overlaps(newRoom))
                {
                    contact = true;
                    break;
                }
            }

            if (!contact)
            {
                rooms.Add(newRoom);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var room in rooms)
        {
            
            Gizmos.DrawWireCube(room.center, room.size);
        }
    }
}
