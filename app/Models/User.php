<?php

namespace App\Models;

use Illuminate\Contracts\Auth\MustVerifyEmail;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Illuminate\Notifications\Notifiable;

class User extends Authenticatable
{
    use HasFactory, Notifiable;

    /**
     * The attributes that are mass assignable.
     *
     * @var array
     */
    protected $fillable = [
        'name',
        'lat',
        'lon',
        'sex',
        'date_birth',
        'created_at',
        'comment',
        'comment_positively'
    ];

    public function coorUsers()
    {
        return $this->hasMany(CoorUsers::class);
    }

    public function questionsAnswersUsers()
    {
        return $this->hasMany(QuestionsAnswersUser::class);
    }
}
