<?php

namespace App\Http\Controllers;

use App\Models\User;
use Illuminate\Http\Request;

class UserController extends Controller
{
    public function newUser(Request $request)
    {
        $user = User::create([
            'name' => $request->name,
            'lat' => $request->lat,
            'lon' => $request->lon,
            'sex' => $request->sex,
            'date_birth' => $request->age,
        ]);

        return response()->json(['user_id' => $user->id]);
    }
}
