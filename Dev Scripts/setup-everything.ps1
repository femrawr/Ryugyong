$origin = Get-Location

try {
    if ((Get-Location).Path -like "*Dev Scripts*") {
        cd ..\
    }

    cd .\Builder\

    npm install

    cd ..\..\
}
finally {
    Set-Location $origin
}