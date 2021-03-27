<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class CoorUsers extends Model
{
    use HasFactory;

    protected $fillable = [
        'lat',
        'lon',
        'user_id',
        'number'
    ];

    public function user()
    {
        return $this->belongsTo(User::class);
    }
}
