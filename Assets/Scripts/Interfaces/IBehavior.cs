﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehavior
{
    bool Act(Monster monster, CommandSystem commandSystem);
}
