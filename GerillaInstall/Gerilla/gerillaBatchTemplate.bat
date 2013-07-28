:  EnergyPlus Batch File for EP-Launch Program 
:  Created on: 8 Mar 2000 by J. Glazer
:  Based on:   RunE+.bat 17 AUG 99 by M.J. Witte
:  Revised:    17 Jul 2000, Linda Lawrie (beta 3 release)
:              27 Sep 2000, Witte/Glazer - add saves for EP-Macro results
:              17 Oct 2000, Lawrie - remove wthrfl option (BLAST weather)
:              09 Feb 2001, Lawrie - Add siz and mtr options, use 3dl and sln files
:              08 Aug 2001, Lawrie - add option for epinext environment variable
:              09 Oct 2001, Lawrie - put in explanation of epinext environment variable
:                                  - also add eplusout.cif for Comis Input Report
:              05 Dec 2001, Lawrie - add new eplusout.bnd (Branch/Node Details)
:              17 Dec 2001, Glazer - explain eptype for no weather case
:              19 Dec 2001, Lawrie - add eplusout.dbg, eplusout.trn, eplusmtr.csv
:                                  - also create eplusmtr.csv 
:              20 Sep 2002, Witte  - Delete all "pausing" stops except the one right
:                                    after EnergyPlus.exe
:              18 Feb 2003, Lawrie - change name of audit.out to eplusout.audit and save it
:              29 Jul 2003, Glazer - add tabular report file handling
:              21 Aug 2003, Lawrie - add "map" file handling
:              21 Aug 2003, Lawrie - change to "styled" output for sizing, map and tabular files
:              29 Aug 2003, Glazer - delete old .zsz and .ssz files if present
:               8 Sep 2003, Glazer - add ReadVars txt and tab outputs
:                                    unlimited columns option
:              09 Feb 2004, Lawrie - add DElight files
:              30 Mar 2004, Glazer - get rid of TRN file
:              30 Jul 2004, Glazer - added use of epout variable as part of groups in ep-launch
:              22 Feb 2005, Glazer - added ExpandObjects preprocessor
:              06 Jun 2006, Lawrie - remove cfp file, add shd file
:              21 Aug 2006, Lawrie - add wrl file
:              22 Aug 2006, Glazer - add convertESOMTR
:              27 Nov 2006, Lawrie - add mdd file
:              20 Feb 2007, Glazer - add csvProc
:              03 Mar 2008, Glazer - add weather stat file copying
:              04 Sep 2008, Lawrie - add sql output
:              09 Jun 2009, Griffith - add edd output (for EMS)
:              05 Feb 2010, Glazer - add basement and slab integration
:              02 Aug 2010, Lawrie - add DFS (daylighting factors) output
:              31 Aug 2010, Glazer - uses local temporary directory for use in multiple processors              
:
:  This batch file can execute EnergyPlus using either command line parameters
:  or environmental variables. The most common method is to use environmental
:  variables but for some systems that do not allow the creation of batch files
:  the parameter passing method should be used.   When not passing parameters the
:  environmental variables are set as part of RUNEP.BAT file and are shown below
: 
:  The batch file should be called from the temporary working directory location.
:
:  When passing parameters instead the first parameter (%1) should be PARAM and the other
:  parameters are shown in the list.
:
:        %epin% or %1 contains the file with full path and no extensions for input files
:
:        %epout% or %2 contains the file with full path and no extensions for output files
:
:        %epinext% or %3 contains the extension of the file selected from the EP-Launch
:          program.  Could be imf or idf -- having this parameter ensures that the
:          correct user selected file will be used in the run.
:
:        %epwthr% or %4 contains the file with full path and extension for the weather file
:         
:        %eptype% or %5 contains either "EP" or "NONE" to indicate if a weather file is used
:
:        %pausing% or %6 contains Y if pause should occur between major portions of
:          batch file
:
:        %maxcol% or %7 contains "250" if limited to 250 columns otherwise contains
:                 "nolimit" if unlimited (used when calling readVarsESO)
:
:        %convESO% or %8 contains Y if convertESOMTR program should be called
:
:        %procCSV% or %9 contains Y if csvProc program should be called
:
:        
:  This batch file is designed to be used only in the EnergyPlus directory that
:  contains the EnergyPlus.exe, Energy+.idd and Energy+.ini files.
:
:  EnergyPlus requires the following files as input:
:
:       Energy+.ini   - ini file with path to idd file (blank = current dir)
:       Energy+.idd   - input data dictionary
:       In.idf        - input data file (must be compatible with the idd version)
:
:       In.epw        - EnergyPlus format weather file
:
:  EnergyPlus will create the following files:
: 
:       Eplusout.audit-  Echo of input (Usually without echoing IDD)
:       Eplusout.end  -  A one line synopsis after the run (success/fail)
:       Eplusout.err  -  Error file
:       Eplusout.eso  -  Standard output file
:       Eplusout.eio  -  One time output file
:       Eplusout.rdd  -  Report variable data dictionary
:       Eplusout.dxf  -  DXF (from report, Surfaces, DXF;)
:       Eplusout.mtr  -  Meter output (similar to ESO format)
:       Eplusout.mtd  -  Meter Details (see what variable is on what meter)
:       Eplusout.bnd  -  Branch/Node Details Report File
:       Eplusout.dbg  -  A debugging file -- see Debug Output object for description
:       Others -- see "Clean up Working Directory for current list"
:
:  The Post Processing Program (PPP) requires the following files as input:
:
:       Eplusout.inp  -  PPP command file (specifies input and output files)
:         This file name is user specified as standard input on the command line  
:         but we will standardize on Eplusout.inp (optional)
:
:       Eplusout.eso  -  Input data (the standard output file from EnergyPlus)
:         This file name is user specified in Eplusout.inp but we will 
:         standardize on Eplusout.eso (can also accept the eplusout.mtr file)
:                        
:
:  The Post Processing Program produces the following output files:
:
:       Eplusout.csv  -  PPP output file in CSV format
:         This file name is user specified in Eplusout.inp but we will 
:         standardize on Eplusout.csv
:
:  This batch file will perform the following steps:
:
:   1.  Clean up directory by deleting old working files from prior run
:   2.  Copy %1.idf (input) into In.idf (or %1.imf to in.imf)
:   3.  Copy %2 (weather) into In.epw
:       Run the Basement preprocessor program if necessary
:       Run the Slab preprocessor program if necessary
:   4.  Execute EnergyPlus
:   5.  If available Copy %1.rvi (post processor commands) into Eplusout.inp
:       If available Copy %1.mvi (post processor commands) into eplusmtr.inp
:       or create appropriate input to get meter output from eplusout.mtr
:   6.  Execute ReadVarsESO.exe (the Post Processing Program)
:       Execute ReadVarsESO.exe (the Post Processing Program) for meter output
:   7.  Copy Eplusout.* to %1.*
:   8.  Clean up directory.
:
: The EPL-RUN.BAT file should be may be either located in the same directory 
: as EnergyPlus.exe or in the temporary directory. If the batch file is in the same
: directory as EnergyPlus.exe the program path is the same as the location 
: the batch file. If the batch file is in the temporary directory the path 
: for the location of EnergyPlus.exe will be passed as a parameter named epPath.

cd\EnergyPlusV8-0-0\
 SET program_path=C:\EnergyPlusV8-0-0\
:pause
 IF "C:\EnergyPlusV8-0-0\" NEQ "" SET program_path=C:\EnergyPlusV8-0-0\
: Set flag if the current directory is the same directory that EnergyPlus and the 
: batch file are located.
:pause
 SET inEPdir=FALSE
 IF "%program_path%"=="C:\EnergyPlusV8-0-0\" SET inEPdir=TRUE
echo Current Drive = %~d0
echo Current Path = %~p0
: pause

: Set the variables if a command line is used
:IF "%9"=="" GOTO skipSetParams
SET epin=%~1
SET epout=%~2
SET epinext=idf
SET epwthr=%~4
SET eptype=EP
SET pausing=Y
SET maxcol=250
SET convESO=Y
SET procCSV=Y
SET output_path=%~~10

SET post_proc=%program_path%PostProcess\
 
:if %pausing%==Y pause

:skipSetParams
:
:  1. Clean up working directory
IF EXIST eplusout.inp   DEL eplusout.inp
IF EXIST eplusout.end   DEL eplusout.end
IF EXIST eplusout.eso   DEL eplusout.eso
IF EXIST eplusout.rdd   DEL eplusout.rdd
IF EXIST eplusout.mdd   DEL eplusout.mdd
IF EXIST eplusout.dbg   DEL eplusout.dbg
IF EXIST eplusout.eio   DEL eplusout.eio
IF EXIST eplusout.err   DEL eplusout.err
IF EXIST eplusout.dxf   DEL eplusout.dxf
IF EXIST eplusout.csv   DEL eplusout.csv
IF EXIST eplusout.tab   DEL eplusout.tab
IF EXIST eplusout.txt   DEL eplusout.txt
IF EXIST eplusmtr.csv   DEL eplusmtr.csv
IF EXIST eplusmtr.tab   DEL eplusmtr.tab
IF EXIST eplusmtr.txt   DEL eplusmtr.txt
IF EXIST eplusout.sln   DEL eplusout.sln
IF EXIST epluszsz.csv   DEL epluszsz.csv
IF EXIST epluszsz.tab   DEL epluszsz.tab
IF EXIST epluszsz.txt   DEL epluszsz.txt
IF EXIST eplusssz.csv   DEL eplusssz.csv
IF EXIST eplusssz.tab   DEL eplusssz.tab
IF EXIST eplusssz.txt   DEL eplusssz.txt
IF EXIST eplusout.mtr   DEL eplusout.mtr
IF EXIST eplusout.mtd   DEL eplusout.mtd
IF EXIST eplusout.bnd   DEL eplusout.bnd
IF EXIST eplusout.dbg   DEL eplusout.dbg
IF EXIST eplusout.sci   DEL eplusout.sci
IF EXIST eplusmap.csv   DEL eplusmap.csv
IF EXIST eplusmap.txt   DEL eplusmap.txt
IF EXIST eplusmap.tab   DEL eplusmap.tab
IF EXIST eplustbl.csv   DEL eplustbl.csv
IF EXIST eplustbl.txt   DEL eplustbl.txt
IF EXIST eplustbl.tab   DEL eplustbl.tab
IF EXIST eplustbl.htm   DEL eplustbl.htm
IF EXIST eplusout.log   DEL eplusout.log
IF EXIST eplusout.svg   DEL eplusout.svg
IF EXIST eplusout.shd   DEL eplusout.shd
IF EXIST eplusout.wrl   DEL eplusout.wrl
IF EXIST eplusout.delightin   DEL eplusout.delightin
IF EXIST eplusout.delightout  DEL eplusout.delightout
IF EXIST eplusout.delighteldmp  DEL eplusout.delighteldmp
IF EXIST eplusout.delightdfdmp  DEL eplusout.delightdfdmp
IF EXIST eplusscreen.csv  DEL eplusscreen.csv
IF EXIST in.imf         DEL in.imf
IF EXIST in.idf         DEL in.idf
IF EXIST in.stat        DEL in.stat
IF EXIST out.idf        DEL out.idf
IF EXIST eplusout.inp   DEL eplusout.inp
IF EXIST in.epw         DEL in.epw
IF EXIST eplusout.audit DEL eplusout.audit
IF EXIST eplusmtr.inp   DEL eplusmtr.inp
IF EXIST test.mvi       DEL test.mvi
IF EXIST expanded.idf   DEL expanded.idf
IF EXIST expandedidf.err   DEL expandedidf.err
IF EXIST readvars.audit   DEL readvars.audit
IF EXIST eplusout.sql   DEL eplusout.sql
IF EXIST sqlite.err  DEL sqlite.err
IF EXIST eplusout.edd  DEL eplusout.edd
IF EXIST eplusout.dfs  DEL eplusout.dfs
IF EXIST slab.int DEL slab.int
IF EXIST BasementGHTIn.idf DEL BasementGHTIn.idf
:if %pausing%==Y pause


:  2. Copy input data file to working directory
IF EXIST "%epout%.epmidf" DEL "%epout%.epmidf"
IF EXIST "%epout%.epmdet" DEL "%epout%.epmdet"
IF NOT EXIST "Energy+.idd" copy "%program_path%Energy+.idd" "Energy+.idd"
IF NOT EXIST "Energy+.ini" copy "%program_path%Energy+.ini" "Energy+.ini"
if "%epinext%" == "" set epinext=idf
if exist "%epin%.%epinext%" copy "%epin%.%epinext%" in.%epinext%
if exist in.imf "%program_path%EPMacro"
if exist out.idf copy out.idf "%epout%.epmidf"
if exist audit.out copy audit.out "%epout%.epmdet"
if exist audit.out erase audit.out
if exist out.idf MOVE out.idf in.idf
if exist in.idf "%program_path%ExpandObjects"
if exist expandedidf.err COPY expandedidf.err eplusout.end
if exist expanded.idf COPY expanded.idf "%epout%.expidf"
if exist expanded.idf MOVE expanded.idf in.idf
if not exist in.idf copy "%epin%.idf" In.idf
:if %pausing%==Y pause


:  3. Test for weather file type and copy to working directory
if %eptype%==EP    copy "%epwthr%" In.epw
: Convert from an extension of .epw to .stat
if %eptype%==EP    SET wthrstat=%epwthr:~0,-4%.stat
if %eptype%==EP    copy "%wthrstat%" in.stat
:if %pausing%==Y pause

: Run the Basement preprocessor program if necessary
IF EXIST "%epin%.bsmt" COPY "%epin%.bsmt" EPObjects.txt
IF EXIST BasementGHTIn.idf DEL EPObjects.txt
IF NOT EXIST BasementGHTIn.idf GOTO :skipBasement
IF EXIST eplusout.end DEL eplusout.end
IF EXIST audit.out DEL audit.out
IF EXIST basementout.audit DEL basementout.audit
IF EXIST "%epout%_bsmt.csv" ERASE "%epout%_bsmt.csv"
IF EXIST "%epout%_bsmt.audit" ERASE "%epout%_bsmt.audit"
IF EXIST "%epout%_bsmt.out" ERASE "%epout%_bsmt.out"
IF EXIST "%epout%_out_bsmt.idf" ERASE "%epout%_out_bsmt.idf"
IF EXIST "%program_path%PreProcess\GrndTempCalc\BasementGHT.idd" COPY "%program_path%PreProcess\GrndTempCalc\BasementGHT.idd" BasementGHT.idd
ECHO Begin Basement Temperature Calculation processing . . . 
"%program_path%PreProcess\GrndTempCalc\Basement.exe"
IF EXIST "MonthlyResults.csv" MOVE "MonthlyResults.csv" "%epout%_bsmt.csv"
IF EXIST "RunINPUT.TXT" MOVE "RunINPUT.TXT" "%epout%_bsmt.out"
IF EXIST "RunDEBUGOUT.txt" MOVE "RunDEBUGOUT.txt" basementout.audit
IF NOT EXIST basementout.audit echo Basement Audit File > basementout.audit
IF EXIST audit.out copy basementout.audit + audit.out basementout.audit
IF EXIST "eplusout.err" copy basementout.audit + eplusout.err basementout.audit
IF EXIST basementout.audit MOVE basementout.audit "%epout%_bsmt.audit"
IF EXIST eplusout.end DEL eplusout.end
IF EXIST audit.out DEL audit.out
IF EXIST basementout.audit DEL basementout.audit
IF EXIST EPObjects.txt COPY EPObjects.txt "%epin%.bsmt"
IF EXIST BasementGHTIn.idf DEL BasementGHTIn.idf
IF EXIST BasementGHT.idd DEL BasementGHT.idd
:skipBasement
IF EXIST EPObjects.txt COPY in.idf+EPObjects.txt in.idf /B
IF EXIST EPObjects.txt COPY "%epout%.expidf"+EPObjects.txt "%epout%.expidf" /B
IF EXIST EPObjects.txt DEL EPObjects.txt

: Run the Slab preprocessor program if necessary
IF EXIST "%epin%.slab" COPY "%epin%.slab" SLABSurfaceTemps.TXT
IF EXIST GHTIn.idf DEL SLABSurfaceTemps.TXT
IF NOT EXIST GHTIn.idf GOTO :skipSlab
IF EXIST eplusout.end DEL eplusout.end
IF EXIST SLABINP.TXT DEL SLABINP.TXT
IF EXIST "SLABSplit Surface Temps.TXT" DEL "SLABSplit Surface Temps.TXT"
IF EXIST audit.out DEL audit.out
IF EXIST "%epout%_slab.ger" ERASE "%epout%_slab.ger"
IF EXIST "%program_path%PreProcess\GrndTempCalc\SlabGHT.idd" COPY "%program_path%PreProcess\GrndTempCalc\SlabGHT.idd" SlabGHT.idd
ECHO Begin Slab Temperature Calculation processing . . . 
"%program_path%PreProcess\GrndTempCalc\Slab.exe"
IF EXIST eplusout.err MOVE eplusout.err "%epout%_slab.ger"
IF EXIST SLABINP.TXT MOVE SLABINP.TXT "%epout%_slab.out"
IF EXIST eplusout.end DEL eplusout.end
IF EXIST "SLABSplit Surface Temps.TXT" DEL "SLABSplit Surface Temps.TXT"
IF EXIST audit.out DEL audit.out
IF EXIST SLABSurfaceTemps.TXT COPY SLABSurfaceTemps.TXT "%epin%.slab"
IF EXIST GHTIn.idf DEL GHTIn.idf
IF EXIST SlabGHT.idd DEL SlabGHT.idd
:skipSlab
IF EXIST SLABSurfaceTemps.TXT COPY in.idf+SLABSurfaceTemps.TXT in.idf /B
IF EXIST SLABSurfaceTemps.TXT COPY "%epout%.expidf"+SLABSurfaceTemps.TXT "%epout%.expidf" /B
IF EXIST SLABSurfaceTemps.TXT DEL SLABSurfaceTemps.TXT
:if %pausing%==Y pause


:  4. Execute EnergyPlus
"%program_path%EnergyPlus"
:if %pausing%==Y pause


:  5. Copy Post Processing Program command file(s) to working directory
IF EXIST "%epin%.rvi" copy "%epin%.rvi" Eplusout.inp
IF EXIST "%epin%.mvi" copy "%epin%.mvi" Eplusmtr.inp
:if %pausing%==Y pause

:  6&8. Copy Post Processing Program command file(s) to working directory
 IF EXIST "%input_path%%~1.rvi" copy "%input_path%%~1.rvi" eplusout.inp
 IF EXIST "%input_path%%~1.mvi" copy "%input_path%%~1.mvi" eplusmtr.inp

:  7&9. Run Post Processing Program(s)
if %maxcol%==250     SET rvset=
if %maxcol%==nolimit SET rvset=unlimited
: readvars creates audit in append mode.  start it off
echo %date% %time% ReadVars >readvars.audit

IF EXIST eplusout.inp %post_proc%ReadVarsESO.exe eplusout.inp %rvset%
IF NOT EXIST eplusout.inp %post_proc%ReadVarsESO.exe " " %rvset%
IF EXIST eplusmtr.inp %post_proc%ReadVarsESO.exe eplusmtr.inp %rvset%
IF NOT EXIST eplusmtr.inp echo eplusout.mtr >test.mvi
IF NOT EXIST eplusmtr.inp echo eplusmtr.csv >>test.mvi
IF NOT EXIST eplusmtr.inp %post_proc%ReadVarsESO.exe test.mvi %rvset%
IF EXIST eplusout.bnd %post_proc%HVAC-Diagram.exe

:  10. Move output files to output path
 IF EXIST eplusout.eso MOVE eplusout.eso "%output_path%%~1.eso"
 IF EXIST eplusout.rdd MOVE eplusout.rdd "%output_path%%~1.rdd"
 IF EXIST eplusout.mdd MOVE eplusout.mdd "%output_path%%~1.mdd"
 IF EXIST eplusout.eio MOVE eplusout.eio "%output_path%%~1.eio"
 IF EXIST eplusout.err MOVE eplusout.err "%output_path%%~1.err"
 IF EXIST eplusout.dxf MOVE eplusout.dxf "%output_path%%~1.dxf"
 IF EXIST eplusout.csv MOVE eplusout.csv "%output_path%%~1.csv"
 IF EXIST eplusout.tab MOVE eplusout.tab "%output_path%%~1.tab"
 IF EXIST eplusout.txt MOVE eplusout.txt "%output_path%%~1.txt"
 IF EXIST eplusmtr.csv MOVE eplusmtr.csv "%output_path%%~1Meter.csv"
 IF EXIST eplusmtr.tab MOVE eplusmtr.tab "%output_path%%~1Meter.tab"
 IF EXIST eplusmtr.txt MOVE eplusmtr.txt "%output_path%%~1Meter.txt"
 IF EXIST eplusout.sln MOVE eplusout.sln "%output_path%%~1.sln"
 IF EXIST epluszsz.csv MOVE epluszsz.csv "%output_path%%~1Zsz.csv"
 IF EXIST epluszsz.tab MOVE epluszsz.tab "%output_path%%~1Zsz.tab"
 IF EXIST epluszsz.txt MOVE epluszsz.txt "%output_path%%~1Zsz.txt"
 IF EXIST eplusssz.csv MOVE eplusssz.csv "%output_path%%~1Ssz.csv"
 IF EXIST eplusssz.tab MOVE eplusssz.tab "%output_path%%~1Ssz.tab"
 IF EXIST eplusssz.txt MOVE eplusssz.txt "%output_path%%~1Ssz.txt"
 IF EXIST eplusout.mtr MOVE eplusout.mtr "%output_path%%~1.mtr"
 IF EXIST eplusout.mtd MOVE eplusout.mtd "%output_path%%~1.mtd"
 IF EXIST eplusout.bnd MOVE eplusout.bnd "%output_path%%~1.bnd"
 IF EXIST eplusout.dbg MOVE eplusout.dbg "%output_path%%~1.dbg"
 IF EXIST eplusout.sci MOVE eplusout.sci "%output_path%%~1.sci"
 IF EXIST eplusmap.csv MOVE eplusmap.csv "%output_path%%~1Map.csv"
 IF EXIST eplusmap.tab MOVE eplusmap.tab "%output_path%%~1Map.tab"
 IF EXIST eplusmap.txt MOVE eplusmap.txt "%output_path%%~1Map.txt"
 IF EXIST eplusout.audit MOVE eplusout.audit "%output_path%%~1.audit"
 IF EXIST eplustbl.csv MOVE eplustbl.csv "%output_path%%~1Table.csv"
 IF EXIST eplustbl.tab MOVE eplustbl.tab "%output_path%%~1Table.tab"
 IF EXIST eplustbl.txt MOVE eplustbl.txt "%output_path%%~1Table.txt"
 IF EXIST eplustbl.htm MOVE eplustbl.htm "%output_path%%~1Table.html"
 IF EXIST eplusout.delightin MOVE eplusout.delightin "%output_path%%~1DElight.in"
 IF EXIST eplusout.delightout  MOVE eplusout.delightout "%output_path%%~1DElight.out"
 IF EXIST eplusout.delighteldmp  MOVE eplusout.delighteldmp "%output_path%%~1DElight.eldmp"
 IF EXIST eplusout.delightdfdmp  MOVE eplusout.delightdfdmp "%output_path%%~1DElight.dfdmp"
 IF EXIST eplusout.svg MOVE eplusout.svg "%output_path%%~1.svg"
 IF EXIST eplusout.shd MOVE eplusout.shd "%output_path%%~1.shd"
 IF EXIST eplusout.wrl MOVE eplusout.wrl "%output_path%%~1.wrl"
 IF EXIST eplusscreen.csv MOVE eplusscreen.csv "%output_path%%~1Screen.csv"
 IF EXIST expandedidf.err copy expandedidf.err+eplusout.err "%output_path%%~1.err"
 IF EXIST readvars.audit MOVE readvars.audit "%output_path%%~1.rvaudit"
 IF EXIST eplusout.sql MOVE eplusout.sql "%output_path%%~1.sql"
 IF EXIST eplusout.edd MOVE eplusout.edd "%output_path%%~1.edd"
 IF EXIST eplusout.dfs MOVE eplusout.dfs "%output_path%%~1DFS.csv"
 
:   11.  Clean up directory.
 ECHO Removing extra files . . .
 IF EXIST eplusout.inp DEL eplusout.inp
 IF EXIST eplusmtr.inp DEL eplusmtr.inp
 IF EXIST in.idf       DEL in.idf
 IF EXIST in.imf       DEL in.imf
 IF EXIST in.epw       DEL in.epw
 IF EXIST in.stat      DEL in.stat
 IF EXIST eplusout.dbg DEL eplusout.dbg
 IF EXIST test.mvi DEL test.mvi
 IF EXIST expandedidf.err DEL expandedidf.err
 IF EXIST readvars.audit DEL readvars.audit
 IF EXIST sqlite.err  DEL sqlite.err

 IF "%inEPdir%"=="FALSE" DEL Energy+.idd
 IF "%inEPdir%"=="FALSE" DEL Energy+.ini
 
 :done
 echo ===== %0 %~1 ===== Complete =====
