# FiniteGroupCalc
This is a project that aims at caclulating certain properties of finite groups.

Currently, it supports growth calculation and some statistics on random walks on groups.

# Main Features

* Persistent caching of what has already been calculated. You don't need to wait for the same values whenever you re-run the process.
See the cachers code in `PersistableCachers` folder for details.
* Abstraction of the group type. To perform calculations over your group, you need to implement the `UlongProcessor` abstract class (see `UlongProcessors.Base.UlongProcessor`).

# Supported groups
* Triangular matrices over ℤₙ
* Heisenberg group over ℤₙ
* Permutation group Sₙ


# Providing your own group
To calculate some finite group's growth, you will need to provide

* `GetIth(int i)`{:.language-cs} to get the i-th element of the group as a ulong
* `Product(ulong a, ulong b)`{:.language-cs} to get the product of two elements (not indices!)
* `GetStandardBasis()`{:.language-cs} to get the standard set of generators of the group

See `UlongProcessors.Base.UlongProcessor` for details.

# Examples
See the `UlongProcessors` folder for examples.

# Warning
This is beta. Which is only slightly beta than nothin. Do check and double-check before relying on the results obtained here.

# License
This project is licensed under the MIT License. Use it as you wish, I don't mind.

# Thanks
* OxyPlot for their plotting library
* Fody for their weavers, and the PropertyChanged weaver in particular
* Newtonsoft.Json for their JSON library