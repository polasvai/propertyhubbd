$divisionsUrl = "https://raw.githubusercontent.com/nuhil/bangladesh-geocode/master/divisions/divisions.json"
$districtsUrl = "https://raw.githubusercontent.com/nuhil/bangladesh-geocode/master/districts/districts.json"
$upazilasUrl = "https://raw.githubusercontent.com/nuhil/bangladesh-geocode/master/upazilas/upazilas.json"

function Get-Json($url) {
    try {
        $response = Invoke-RestMethod -Uri $url
        # If the response is wrapped (like the divisions example seen), unwrap it
        if ($response -is [array] -and $response[0].type -eq "header") {
             # It's the wrapped format from the earlier view_url output
             # Find the table data
             $table = $response | Where-Object { $_.type -eq "table" }
             return $table.data
        }
        # If it's a direct list (often the case for raw JSON if not wrapped in that specific format)
        # We need to handle both potential formats since the first file view showed a complex structure
        # but raw github often is just the array.
        # Let's inspect the object type.
        if ($response.data) { return $response.data } # Common API wrapper
        if ($response[2].data) { return $response[2].data } # The structure seen in view_url
        return $response
    }
    catch {
        Write-Error "Failed to fetch $url"
        return $null
    }
}

$divisions = Get-Json $divisionsUrl
$districts = Get-Json $districtsUrl
$upazilas = Get-Json $upazilasUrl

# Build Hierarchy
$hierarchy = @{}

foreach ($div in $divisions) {
    $divId = $div.id
    $divName = $div.name
    $hierarchy[$divId] = @{
        Name = $divName
        Districts = @{}
    }
}

foreach ($dist in $districts) {
    $divId = $dist.division_id
    $distId = $dist.id
    $distName = $dist.name
    
    if ($hierarchy.ContainsKey($divId)) {
        $hierarchy[$divId].Districts[$distId] = @{
            Name = $distName
            Upazilas = @()
        }
    }
}

foreach ($upz in $upazilas) {
    $distId = $upz.district_id
    $upzName = $upz.name
    
    # scan all divisions to find the district
    foreach ($divId in $hierarchy.Keys) {
        if ($hierarchy[$divId].Districts.ContainsKey($distId)) {
            $hierarchy[$divId].Districts[$distId].Upazilas += $upzName
            break
        }
    }
}

# Generate C# Code
$sb = [System.Text.StringBuilder]::new()
$sb.AppendLine('            var locationData = new Dictionary<string, Dictionary<string, string[]>>')
$sb.AppendLine('            {')

foreach ($divKey in $hierarchy.Keys) {
    $div = $hierarchy[$divKey]
    $divName = $div.Name
    $sb.AppendLine("                { `"$divName`", new Dictionary<string, string[]> {")
    
    foreach ($distKey in $div.Districts.Keys) {
        $dist = $div.Districts[$distKey]
        $distName = $dist.Name
        $upzList = $dist.Upazilas | ForEach-Object { "`"$_`"" }
        $upzString = $upzList -join ", "
        $sb.AppendLine("                    { `"$distName`", new[] { $upzString } },")
    }
    
    $sb.AppendLine("                }},")
}
$sb.AppendLine("            };")

$sb.ToString() | Out-File "generated_location_data.txt" -Encoding UTF8
Write-Host "Generation Complete. Data saved to generated_location_data.txt"
