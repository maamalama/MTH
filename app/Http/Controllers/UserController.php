<?php

namespace App\Http\Controllers;

use App\Models\Questions;
use App\Models\QuestionsAnswers;
use App\Models\User;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Http;

class UserController extends Controller
{
    public function newUser(Request $request)
    {
        $user = User::create([
            'name' => $request->name,
            'sex' => $request->sex,
            'date_birth' => date('Y-m-d',strtotime($request->age)),
        ]);

        return response()->json(['user_id' => $user->id]);
    }

    public function getUsers()
    {
        return response()->json(['users' => User::all()]);
    }

    public function updateUserLonLat(Request $request, User $user)
    {
        $user->update([
            'lat' => $request->lat,
            'lon' => $request->lon,
        ]);

        return response()->json();
    }

    public function lastUpdate(Request $request, User $user)
    {
        $respone = Http::get('',[
            'comment' => $request->comment
        ])->json()['result'];

        $user->update([
            'comment' => $request->comment,
            'comment_positively' => $respone
        ]);

        return response()->json();
    }

    public function test()
    {
        return response()->json(['date' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->get()]);
    }
}
