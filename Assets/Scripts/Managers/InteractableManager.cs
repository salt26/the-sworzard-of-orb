﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour {
    
    // interactables에 있는 상호작용 가능 개체 프리팹의 순서가 각 상호작용 가능 개체의 ID가 됩니다.
    // (0부터 시작)
    public List<Interactable> interactables = new List<Interactable>();
}
