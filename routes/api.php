<?php

use App\Http\Controllers\CoorUsers;
use App\Http\Controllers\QuestionsController;
use App\Http\Controllers\UserController;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::post('user', [UserController::class, 'newUser']);
Route::post('answer-user', [QuestionsAnswersUserController::class, 'create']);
Route::get('user', [UserController::class, 'getUsers']);
Route::put('user-last-update', [UserController::class, 'lastUpdate']);
Route::get('quest/{user}', [QuestionsController::class, 'getQuestions']);
Route::get('quest', [QuestionsController::class, 'getFirstQuestions']);
Route::put('user/{user}', [UserController::class, 'updateUserLonLat']);
Route::get('test', [UserController::class, 'test']);
Route::post('coor-users', [CoorUsers::class, 'create']);
