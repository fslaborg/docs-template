namespace testProject

///<summary>a module</summary>
module Say =
    /// <summary>a function that says hello to the name you pass it</summary>
    /// <param name="name">the name to say hello to</param>
    /// <returns>says hello to the passed name via console</returns>
    let hello name =
        printfn "Hello %s" name
