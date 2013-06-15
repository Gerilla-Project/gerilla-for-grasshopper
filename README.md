Gerilla for Grasshopper, v0.9, 2013-06-11
====================================================

*Please be advised that Gerilla is still in early development.*

Gerilla is a plugin for Grasshopper / Rhino that will allow you to run full building energy simulations on using the EnergyPlus simulation engine. It was born in the Product Architecture Lab at Stevens Institute of Technology in Fall of 2011 and is the result of the collaborative efforts of Michael Marvin, Drew Orvieto, and Ben Silverman. Brendan Albano joined the development team in 2013 and is organizing Gerilla's public launch. Gerilla is an open source, free to download plugin for Grasshopper.

Development Status
------------------

After its initial development in 2011, the Gerilla project was dormant until restarting development in 2013. Gerilla was initally written for Energy Plus v6, and an older version of Grasshopper. The current code (as of 2013-06-15) has been preliminarily updated for Rhino 4, EnergyPlus v8 and Grasshopper v0.9.0014, but needs significantly more testing. That's where you come in!

The Gerilla project is programmed in VB.net and uses the SharpDevelop IDE.

Gameplan
--------
The goal this summer is to get Gerilla in a place where it is useable and really easy for more people to get involved with its development, so that it can take on a life of its own. In order to do that, we need to:
- Clean up the code and add comments.
- Write up detailed instructions for contributing to the project and compile a list of links to resources to help contributors who are new to GitHub, EnergyPlus, or developing plug-ins for Grasshopper.
- Get Gerilla up and running on Rhino 5 and the latest version of Grasshopper.
- Establish a protocol of verifying the EnergyPlus results generated from Gerilla by comparying them with those from another program that uses EnergyPlus.
- Set some goals for the future of the project.

Contributing
------------
Get involved! The Gerilla project is not just about creating a product, but also about architectural tool-making and learning-by-doing. Everyone is welcome.

If you encounter a bug or have an idea for an awesome new feature, submit a new issue! For bugs, please include as much detail as you can, including the version of your operating system, Rhino, Grasshopper and Energy Plus, as well as step by step instructions to reproduce the bug.

Even better than just submitting an issue is fixing the bug or creating the feature yourself! First off, submit an issue letting us know what you're working on, then [fork the project](https://help.github.com/articles/fork-a-repo), code it up, and submit a [pull request](https://help.github.com/articles/using-pull-requests).

License
-------

Copyright (c) 2011, 2013  Michael Marvin (michaeljmarvin@gmail.com), Drew Orvieto (dorvieto@gmail.com), Ben Silverman (bhsilverman@gmail.com), Brendan Albano (brendan@brendanalbano.com)

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see [http://www.gnu.org/licenses/](http://www.gnu.org/licenses).

The developers of Gerilla grant you permission to link this plugin to McNeel's Rhino/Grasshopper environment, which is a non-free program. This is potentially an exception to the GPL: [https://www.gnu.org/licenses/gpl-faq.html#GPLPluginsInNF](https://www.gnu.org/licenses/gpl-faq.html#GPLPluginsInNF)

Installation
------------

The install files are ound in the "GerillaInstall" folder.

1. Gerilla requires EnergyPlus Version 8.
2. Copy the "Gerilla.gha" file into your Grasshopper components folder. 
3. Copy the "Gerilla" folder to your C: drive. The file path should be: C:\Gerilla. 

Additional directions are contained in the Gerilla primer.

Tested with
-----------
Rhinocerous v4.0 SR9, Grasshopper v0.9.0014, EnergyPlus v8.0.0: 2013-06-11, works. 