using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class getInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void updateFile()
    {
        List<string> savelist = new List<string>();

        File.WriteAllLines(@"input.txt", savelist);
    }
}
