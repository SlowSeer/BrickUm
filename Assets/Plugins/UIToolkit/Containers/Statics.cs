using UnityEngine;
using System.Collections;

public class Statics {
    private static Statics instance;

    public Rect myRect = new Rect(10, 10, 10, 10);
    public string myString = "BOX";

    private Statics() { }

    public static Statics Instance {
        get {
            if (instance == null) {
                instance = new Statics();
            }
            return instance;
        }
    }
}
