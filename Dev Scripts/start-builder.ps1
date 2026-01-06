$origin = Get-Location

try {
    if ((Get-Location).Path -like "*Dev Scripts*") {
        cd ..\
    }

    cd .\Builder\

    node .\server\index.js

    cd ..\
}
finally {
    Set-Location $origin
}