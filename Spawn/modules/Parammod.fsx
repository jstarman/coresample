#r @"tools/FAKE.4.62.5/tools/FakeLib.dll"
open Fake
let raiseIfMissingParam (name,value) =
    if (isNullOrEmpty value)
    then
        raise (System.ArgumentException(sprintf "Please pass in the following param: %s" name))

let getNugetPackageVersion gitHash = 
    if gitHash <> "" then        
        let shortHash () =  
            if gitHash.Length > 16 then 
                gitHash.[0..16]                 
            else 
                gitHash
        let currentDate = System.DateTime.Now.ToString("yyyy.M.d")
        sprintf "%s-git%s" currentDate (shortHash ())
    else
        ""
