// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Cube3D.
// 
// Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media.Animation;
using Cube3D.Effects;

namespace Cube3D.Config
{
    public class Settings
    {
        private int _animationDuration = 500;

        public int AnimationDuration
        {
            get => _animationDuration;
            set
            {
                if ( value < Const.AnimationDurationMin || value > Const.AnimationDurationMax )
                {
                    _animationDuration = Const.AnimationDurationMin;
                }
                else
                {
                    _animationDuration = value;
                }
            }
        }

        [JsonConverter( typeof( EffectTypeJsonConverter ) )]
        public string SelectedEffect { get; set; } = EffectFactory.Default;

        [JsonConverter( typeof( EaseTypeJsonConverter ) )]
        public string EaseType { get; set; } = EaseFactory.None;

        public EasingMode     EaseMode                         { get; set; } = EasingMode.EaseOut;
        public TransitionType TransitionType                   { get; set; } = TransitionType.AnimationAndNotificationGrid;
        public bool           ShowNotificationGridOnAllScreens { get; set; }
    }

    [Flags]
    public enum TransitionType
    {
        AnimationOnly                = 0b0001,
        NotificationGridOnly         = 0b0010,
        AnimationAndNotificationGrid = AnimationOnly | NotificationGridOnly
    }

    internal sealed class EffectTypeJsonConverter : JsonConverter<string>
    {
        public override string Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            if ( reader.TokenType == JsonTokenType.Number && reader.TryGetInt32( out var index ) )
                return EffectFactory.NameFromLegacyIndex( index );
            return reader.TokenType == JsonTokenType.String
                ? reader.GetString() ?? EffectFactory.Default
                : EffectFactory.Default;
        }

        public override void Write( Utf8JsonWriter writer, string value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value );
        }
    }

    internal sealed class EaseTypeJsonConverter : JsonConverter<string>
    {
        public override string Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            if ( reader.TokenType == JsonTokenType.Number && reader.TryGetInt32( out var index ) )
                return EaseFactory.NameFromLegacyIndex( index );
            return reader.TokenType == JsonTokenType.String
                ? reader.GetString() ?? EaseFactory.None
                : EaseFactory.None;
        }

        public override void Write( Utf8JsonWriter writer, string value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value );
        }
    }
}