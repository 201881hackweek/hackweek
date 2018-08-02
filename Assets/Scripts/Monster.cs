using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonster {
    int Life {
		get;
		set;
	}
	int Speed {
		get;
		set;
	}
	int Damage {
		set;
		get;
	}
}

