<?php

namespace App\Http\Controllers;

use App\Models\CoorUsers as ModelsCoorUsers;
use App\Models\User;
use Illuminate\Http\Request;

class CoorUsersController extends Controller
{
    public function create(Request $request)
    {
        ModelsCoorUsers::create([
            'lat' => $request->lat,
            'lon' => $request->lon,
            'user_id' => $request->user_id,
            'number' => $request->number
        ]);

        return response()->json();
    }

    public function getCoorUsers(User $user)
    {
        return response()->json(['coors' => ModelsCoorUsers::where('number', '!=', null)->where('user_id', $user->id)->select('id','number')->get()]);
    }
}
