<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreatecoorUsersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('coor_users', function (Blueprint $table) {
            $table->id();
            $table->float('lat', 16,8);
            $table->float('lon', 16,8);
            $table->string('number')->nullable();
            $table->timestamps();

            $table->foreignId('user_id')->constrained('users')->onDelete('CASCADE');
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('сoor_users');
    }
}
